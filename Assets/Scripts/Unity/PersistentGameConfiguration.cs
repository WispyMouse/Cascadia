namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.AIs;
    using UnityEngine;
    using System.Threading.Tasks;
    using Cascadia.Unity.UI;
    using UnityEngine.AddressableAssets;

    public class PersistentGameConfiguration : MonoBehaviour
    {
        public delegate void GameplaySettingsUpdated(GameConfiguration newConfiguration);
        public GameplaySettingsUpdated OnGameplaySettingsUpdated;

        public const string HUMAN = "Human";
        public static PersistentGameConfiguration Singleton { get; private set; } = null;

        public GameConfiguration GameConfiguration => this.gameConfiguration;
        private GameConfiguration gameConfiguration;
        public int Width => this.GameConfiguration.Width;
        public int Height => this.GameConfiguration.Height;

        /// <summary>
        /// Connections cannot be made until this many turns have passed.
        /// At zero or below, this setting is disabled.
        /// </summary>
        public int StallTurn => this.GameConfiguration.StallTurn;

        #region Debug Values
        [Header("Debug Values")]

        /// When starting the game from in editor, set <see cref="GameConfiguration.Width"/> to this.
        [Range(3, 12)]
        [SerializeField]
        private int debugWidth = 5;

        /// When starting the game from in editor, set <see cref="GameConfiguration.Height"/> to this.
        [Range(3, 12)]
        [SerializeField]
        private int debugHeight = 5;

        /// When starting the game from in editor, set <see cref="GameConfiguration.PlayerCount"/> to this.
        [Range(BoardState.MINIMUMPLAYERS, BoardState.MAXIMUMPLAYERS)]
        [SerializeField]
        private int debugPlayerCount = 2;

        /// When starting the game from in editor, set <see cref="GameConfiguration.StallTurn"/> to this.
        [Range(0, 8)]
        [SerializeField]
        private int debugStallTurn = 0;

        /// <summary>
        /// For each player (counting up to <see cref="debugPlayerCount"/>), assign them the AICore.
        /// present at the corresponding index in <see cref="AICores"/>.
        /// </summary>
        [SerializeField]
        private List<int> debugAICores = new();
        #endregion

        public IReadOnlyList<PlayerProfile> Players => this.players;
        private readonly List<PlayerProfile> players = new();

        /// <summary>
        /// When a player makes a connection, behind each connection there will be a line.
        /// This line helps the user coordinate their eyes to a knocked out connection.
        /// The <see cref="PlayerProfile.Index"/> corresponds to the Color they will be assigned.
        /// TODO: This should likely be generated *by the emoji used*, not at design time.
        /// </summary>
        [SerializeField]
        private List<Color> knockoutColorsForTurns = new();

        /// <summary>
        /// List of all <see cref="AICore"/>s that can be assigned.
        /// Ordered in the editor. The indexes need to correspond to
        /// other configured index values, so be careful about rearranging.
        /// All of these AIs will appear in the player configuration UI (PlayerConfiguratorSelector)
        /// </summary>
        public IReadOnlyList<AICore> AICores => this.aiCores;

        /// <summary>
        /// List of all <see cref="AICore"/>s that can be assigned.
        /// </summary>
        [SerializeField]
        private List<AICore> aiCores = new();

        public bool ConfigurationLoaded { get; private set; } = false;

        private void Awake()
        {
            // If we already have an instance, don't make another
            if (Singleton != null)
            {
                Destroy(this.gameObject);
                return;
            }

            // This is now the singleton; unparent it so that we can mark as DontDestroyOnLoad
            Singleton = this;
            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);

            this.gameConfiguration = new GameConfiguration(this.debugWidth, this.debugHeight, 0, 3, this.debugStallTurn, forceOrderlyTurn: null);

            int playerCount = BoardState.MINIMUMPLAYERS;

#if UNITY_EDITOR
            playerCount = this.debugPlayerCount;
#endif

            for (int ii = 0; ii < playerCount; ii++)
            {
                PlayerProfile newProfile = this.AddPlayer();

#if UNITY_EDITOR
                // Quick AI core test; assign AI cores to players based on editor-only debug variables
                if (this.debugAICores.Count > ii && this.debugAICores[ii] >= 0)
                {
                    newProfile.AICoreIndex = this.debugAICores[ii];
                    newProfile.IsAI = true;
                    newProfile.PlayerName = this.AICores[this.debugAICores[ii]].name;
                }
#endif
            }
        }

        private async Awaitable Start()
        {
            await Addressables.InitializeAsync().Task;

            // After starting, send up the event that the game configuration has changed
            // This will help UI items decide to update, which should assign in OnEnable
            this.OnGameplaySettingsUpdated?.Invoke(this.gameConfiguration);

            this.ConfigurationLoaded = true;
        }

        public void UpdateWidth(int newValue)
        {
            this.gameConfiguration.Width = newValue;
            this.OnGameplaySettingsUpdated?.Invoke(this.GameConfiguration);
        }

        public void UpdateHeight(int newValue)
        {
            this.gameConfiguration.Height = newValue;
            this.OnGameplaySettingsUpdated?.Invoke(this.GameConfiguration);
        }

        public void UpdateStall(int newValue)
        {
            this.gameConfiguration.StallTurn = newValue;
            this.OnGameplaySettingsUpdated?.Invoke(this.GameConfiguration);
        }

        /// <summary>
        /// Goes through the <see cref="Players"/> list, and removes the specified player.
        /// Every PlayerProfile past that will have its <see cref="PlayerProfile.Index"/> reduced by one.
        /// </summary>
        public void RemovePlayer(PlayerProfile toRemove)
        {
            bool startRemoving = false;

            for (int ii = 0; ii < this.Players.Count; ii++)
            {
                // Reduce the index so that we have a stable 0-N
                if (startRemoving)
                {
                    this.players[ii].Index--;
                    continue;
                }

                // If this is what we're trying to remove, snip it
                if (this.Players[ii] == toRemove)
                {
                    startRemoving = true;
                    this.players.RemoveAt(ii);
                    this.gameConfiguration.PlayerCount = this.players.Count;

                    // Move the iterator back one, since this has removed something
                    ii--;

                    continue;
                }
            }

            this.OnGameplaySettingsUpdated?.Invoke(this.GameConfiguration);
        }

        /// <summary>
        /// Adds a new player. The returned PlayerProfile
        /// will already have the appropriate <see cref="PlayerProfile.Index"/>.
        /// </summary>
        /// <returns></returns>
        public PlayerProfile AddPlayer()
        {
            // Start looking up an icon for them; asynchronous action
            PlayerProfile newProfile = new()
            {
                Index = this.players.Count,
                Sprite = PlayerProfileSprite.GetNeedsResolution()
            };

            SpriteLookup.AssignRandomSpriteFromAddressablesAsync(newProfile);

            newProfile.KnockoutColor = this.knockoutColorsForTurns[newProfile.Index];
            newProfile.IsAI = false;
            newProfile.PlayerName = HUMAN;

            this.players.Add(newProfile);
            this.gameConfiguration.PlayerCount = this.players.Count;

            return newProfile;
        }

        public void ForceSetConfig(GameConfiguration configuration)
        {
            this.gameConfiguration = configuration;

            this.EnsurePlayerCount(configuration.PlayerCount);
        }

        public void HumanitizeForTests()
        {
            this.debugAICores.Clear();
            this.EnsurePlayerCount(BoardState.MINIMUMPLAYERS);
        }

        public PlayerProfile GetPlayerProfile(PlayerSide forSide)
        {
            return this.Players[forSide.PlayerProfileIndex];
        }

        public void EnsurePlayerCount(int size)
        {
            // Remove players over limit
            for (int ii = this.players.Count - 1; ii >= size; ii--)
            {
                this.RemovePlayer(this.players[ii]);
            }

            // Add players until we hit the limit
            for (int ii = this.players.Count; ii < size; ii++)
            {
                this.AddPlayer();
            }
        }
    }
}
