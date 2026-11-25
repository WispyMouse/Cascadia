namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cascadia.AIs;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using static UnityEngine.Audio.GeneratorInstance;

    public class PersistentGameConfiguration : MonoBehaviour
    {
        public delegate void GameplaySettingsUpdatedDelegate(GameConfiguration newConfiguration);
        public GameplaySettingsUpdatedDelegate OnGameplaySettingsUpdated;
        public delegate void ChallengeDefinitionSelectedDelegate(ChallengeDefinition newChallenge, int clearCount);
        public ChallengeDefinitionSelectedDelegate OnChallengeSettingsUpdated;

        public const string HUMAN = "Human";
        public static PersistentGameConfiguration Singleton { get; private set; } = null;

        public GameConfiguration GameConfiguration => this.gameConfiguration;
        private GameConfiguration gameConfiguration;
        public ChallengeDefinition CurrentChallenge => this.currentChallenge;
        private ChallengeDefinition currentChallenge;
        public bool ChallengeIsSet => !ReferenceEquals(this.currentChallenge, null);

        public int Width => this.GameConfiguration.Width;
        public int Height => this.GameConfiguration.Height;

        /// <summary>
        /// Connections cannot be made until this many turns have passed.
        /// At zero or below, this setting is disabled.
        /// </summary>
        public int StallTurn => this.GameConfiguration.StallTurn;

        /// <summary>
        /// All animation times are multiplied by this to determine their duration.
        /// </summary>
        public float AnimationSpeedModifier =>
#if UNITY_INCLUDE_TESTS
                .001f;
#else
                return 1f;
#endif

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
        public IList<AICore> AICores => this.aiCores;

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

            this.gameConfiguration = GameConfiguration.GetDefaultConfiguration();
        }

        private async Awaitable Start()
        {
            // If the singleton was set, and this isn't it, cancel out of the Start
            if (Singleton != this)
            {
                return;
            }

            await Addressables.InitializeAsync().Task;

            // Ensure there are at least enough players to fill out a minimum game
            for (int ii = this.players.Count; ii < BoardState.MINIMUMPLAYERS; ii++)
            {
                this.AddPlayer(true);
            }

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
                    Debug.Log($"{nameof(PersistentGameConfiguration)} {nameof(RemovePlayer)} Removing player {toRemove.PlayerName} at index {ii}");

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
        public PlayerProfile AddPlayer(bool setSprite)
        {
            Debug.Log($"{nameof(PersistentGameConfiguration)} {nameof(AddPlayer)} Adding new player, index {this.players.Count}");

            // Fire-and-forget way of assigning sprites after they're loaded
            static async void AssignAddedPlayerASprite(PlayerProfile playerProfile)
            {
                await SpriteLookup.AssignRandomSpriteFromAddressablesAsync(playerProfile);
            }

            // Start looking up an icon for them; asynchronous action
            PlayerProfile newProfile = new()
            {
                Index = this.players.Count
            };

            if (setSprite)
            {
                // Fire off async void to eventually assign graphic
                // We can know if there isn't an assigned graphic yet by asking PlayerProfile.HasSprite
                AssignAddedPlayerASprite(newProfile);
            }

            newProfile.KnockoutColor = this.knockoutColorsForTurns[newProfile.Index];
            newProfile.SetAICore(null);

            this.players.Add(newProfile);
            this.gameConfiguration.PlayerCount = this.players.Count;

            return newProfile;
        }

        public void ForceSetConfig(GameConfiguration configuration, bool challengeMode = false)
        {
            Debug.Log($"{nameof(PersistentGameConfiguration)} {nameof(ForceSetConfig)} Asked to force set config for custom gameplay.");

            this.gameConfiguration = configuration;

            this.EnsurePlayerCount(configuration.PlayerCount, challengeMode);

            if (challengeMode)
            {
                this.Players[0].SetAICore(null);

                for (int ii = 0; ii < this.currentChallenge.AICores.Count; ii++)
                {
                    PersistentGameConfiguration.Singleton.Players[ii + 1].SetAICore(PersistentGameConfiguration.Singleton.AICores.IndexOf(this.currentChallenge.AICores[ii]), this.currentChallenge.AISprites[ii]);
                }
            }

            this.OnGameplaySettingsUpdated?.Invoke(this.gameConfiguration);
        }

        public void ForceSetConfig(ChallengeDefinition challenge)
        {
            Debug.Log($"{nameof(PersistentGameConfiguration)} {nameof(ForceSetConfig)} Asked to force set config for challenge {challenge.ChallengeName}");

            this.gameConfiguration = challenge.GetGameConfiguration();
            this.EnsurePlayerCount(this.gameConfiguration.PlayerCount, true);

            this.Players[0].SetAICore(null);

            for (int ii = 0; ii < this.currentChallenge.AICores.Count; ii++)
            {
                PersistentGameConfiguration.Singleton.Players[ii + 1].SetAICore(PersistentGameConfiguration.Singleton.AICores.IndexOf(this.currentChallenge.AICores[ii]), this.currentChallenge.AISprites[ii]);
            }

            this.OnGameplaySettingsUpdated?.Invoke(this.gameConfiguration);
        }

        public async Task HumanitizeForTests()
        {
            this.aiCores.Clear();
            this.EnsurePlayerCount(BoardState.MINIMUMPLAYERS);

            bool allResolved = false;
            do
            {
                foreach (PlayerProfile profile in this.players)
                {
                    if (!profile.HasSprite)
                    {
                        await Task.Delay(1);
                        break;
                    }
                }

                allResolved = true;
            } while (!allResolved);
        }

        public PlayerProfile GetPlayerProfile(PlayerSide forSide)
        {
            return this.players[forSide.PlayerProfileIndex];
        }

        public void EnsurePlayerCount(int size, bool setFirstToHuman = false)
        {
            Debug.Log($"{nameof(PersistentGameConfiguration)} {nameof(EnsurePlayerCount)} Ensuring player size is at least {size}");

            int start = 0;
            if (setFirstToHuman && this.players.Any())
            {
                this.players.RemoveRange(1, this.players.Count - 1);
                start = 1;
                this.players[0].SetAICore(null);
            }
            else
            {
                this.players.Clear();
            }

            // Add players until we hit the limit
            for (int ii = start; ii < size; ii++)
            {
                this.AddPlayer(false);
            }
        }

        public int AddAICore(AICore toAdd)
        {
            int nextIndex = this.aiCores.Count;
            Debug.Log($"Adding AICore {toAdd.name} to index {nextIndex}", this);
            this.aiCores.Add(toAdd);
            return nextIndex;
        }

        public void ClearChallenge()
        {
            this.currentChallenge = null;
        }

        public async void SetChallenge(ChallengeDefinition definition)
        {
            this.currentChallenge = definition;

            this.ForceSetConfig(this.currentChallenge);

            this.OnChallengeSettingsUpdated?.Invoke(definition, await SaveGameManager.Singleton.GetClearCount(definition));
        }
    }
}
