namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.AIs;
    using UnityEngine;

    public class PersistentGameConfiguration : MonoBehaviour
    {
        public delegate void GameplaySettingsUpdated();
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

        [Range(3, 12)]
        [SerializeField]
        private int startWidth = 5;

        [Range(3, 12)]
        [SerializeField]
        private int startHeight = 5;

        [Range(BoardState.MINIMUMPLAYERS, 6)]
        [SerializeField]
        private int startPlayerCount = 2;

        [Range(0, 8)]
        [SerializeField]
        private int startStallTurn = 0;

        public IReadOnlyList<PlayerProfile> Players => this.players;
        private readonly List<PlayerProfile> players = new();

        [SerializeField]
        private List<int> coresByIndex = new();

        [SerializeField]
        private List<Color> knockoutColorsForTurns = new();

        public List<Sprite> PlayerIconOptions = new();

        public List<AICore> AICores = new();

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

            this.gameConfiguration = new GameConfiguration(this.startWidth, this.startHeight, this.startPlayerCount, 3, this.startStallTurn, forceOrderlyTurn: null);

            for (int ii = 0; ii < this.startPlayerCount; ii++)
            {
                PlayerProfile newProfile = this.AddPlayer();

#if UNITY_EDITOR
                // Quick AI core test
                if (this.coresByIndex.Count > ii && this.coresByIndex[ii] >= 0)
                {
                    newProfile.AICoreIndex = this.coresByIndex[ii];
                    newProfile.IsAI = true;
                    newProfile.PlayerName = this.AICores[this.coresByIndex[ii]].name;
                }
#endif
            }

            this.OnGameplaySettingsUpdated?.Invoke();
        }

        public void UpdateWidth(int newValue)
        {
            this.gameConfiguration.Width = newValue;
            this.OnGameplaySettingsUpdated?.Invoke();
        }

        public void UpdateHeight(int newValue)
        {
            this.gameConfiguration.Height = newValue;
            this.OnGameplaySettingsUpdated?.Invoke();
        }

        public void UpdateStall(int newValue)
        {
            this.gameConfiguration.StallTurn = newValue;
            this.OnGameplaySettingsUpdated?.Invoke();
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

                    // Move the iterator back one, since this has removed something
                    ii--;

                    continue;
                }
            }

            this.OnGameplaySettingsUpdated?.Invoke();
        }

        /// <summary>
        /// Adds a new player. The returned PlayerProfile
        /// will already have the appropriate <see cref="PlayerProfile.Index"/>.
        /// </summary>
        /// <returns></returns>
        public PlayerProfile AddPlayer()
        {
            PlayerProfile newProfile = new();

            // Find the first index not used by anyone for the icon
            // It'll see if anyone is using the first, and if not, use that;
            // otherwise check the second, etc.
            int iconIndex = 0;
            for (int ii = 0, count = this.PlayerIconOptions.Count; ii < count; ii++)
            {
                // Check each player to see if they're using this index
                bool unused = true;
                for (int jj = 0, jjcount = this.Players.Count; jj < jjcount; jj++)
                {
                    if (this.Players[jj].RepresenterSpriteIndex == ii)
                    {
                        unused = false;
                        break;
                    }
                }
                if (unused)
                {
                    iconIndex = ii;
                    break;
                }
            }

            newProfile.Index = this.players.Count;
            newProfile.RepresenterSpriteIndex = Random.Range(0, this.PlayerIconOptions.Count);
            newProfile.KnockoutColor = this.knockoutColorsForTurns[newProfile.Index];
            newProfile.IsAI = false;
            newProfile.PlayerName = HUMAN;

            this.players.Add(newProfile);

            this.OnGameplaySettingsUpdated?.Invoke();

            return newProfile;
        }

        public void ForceSetConfig(GameConfiguration configuration) => this.gameConfiguration = configuration;

        public void HumanitizeForTests()
        {
            this.coresByIndex.Clear();
            this.players.Clear();

            for (int ii = 0; ii < this.GameConfiguration.PlayerCount; ii++)
            {
                this.AddPlayer();
            }
        }
    }
}
