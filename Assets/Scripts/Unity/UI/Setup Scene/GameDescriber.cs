namespace Cascadia.Unity.UI.SetupScene
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Describes the current game configuration and outputs it in the Game Setup menu.
    /// </summary>
    public class GameDescriber : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private TMP_Text descriptionText;
        [SerializeReference]
        private Image completedSymbol;
        [SerializeReference]
        private DescriberGridPainter describerGridPainter;

        [Header("Data Resources")]
        [SerializeReference]
        private List<Sprite> clearSprites;

        public void OnEnable()
        {
            PersistentGameConfiguration.Singleton.OnGameplaySettingsUpdated += this.UpdateDescription;
            PersistentGameConfiguration.Singleton.OnChallengeSettingsUpdated += this.UpdateDescription;
        }
        public void OnDisable()
        {
            PersistentGameConfiguration.Singleton.OnGameplaySettingsUpdated -= this.UpdateDescription;
            PersistentGameConfiguration.Singleton.OnChallengeSettingsUpdated -= this.UpdateDescription;
        }

        public void UpdateDescriptionFromCurrentConfiguration()
        {
            this.UpdateDescription(PersistentGameConfiguration.Singleton.GameConfiguration);
        }

        public void UpdateDescription(ChallengeDefinition definition, int clearCount)
        {
            StringBuilder description = new();

            description.AppendLine(definition.ChallengeName);

            GameConfiguration configuration = definition.GetGameConfiguration();
            this.descriptionText.text = GenerateTextDescription(configuration, PersistentGameConfiguration.Singleton.Players);
            this.describerGridPainter.Paint(configuration);

            if (clearCount == 0)
            {
                this.completedSymbol.gameObject.SetActive(false);
            }
            else
            {
                this.completedSymbol.gameObject.SetActive(true);
                int clampedIndex = Math.Min(clearCount - 1, this.clearSprites.Count);
                this.completedSymbol.sprite = this.clearSprites[clampedIndex];
            }
        }

        public void UpdateDescription(GameConfiguration newConfiguration)
        {
            this.descriptionText.text = GenerateTextDescription(newConfiguration, PersistentGameConfiguration.Singleton.Players);
            this.describerGridPainter.Paint(newConfiguration);
        }

        public static string GenerateTextDescription(GameConfiguration newConfiguration, IReadOnlyList<PlayerProfile> players)
        {
            StringBuilder description = new();

            description.AppendLine($"{newConfiguration.PlayerCount} players");
            description.AppendLine($"{newConfiguration.Width}x{newConfiguration.Height}");

            if (newConfiguration.StallTurn > 0)
            {
                description.AppendLine($"No connections until turn {newConfiguration.StallTurn}");
            }

            bool appendVs = false;
            foreach (PlayerProfile curPlayer in players)
            {
                if (appendVs)
                {
                    description.Append($" vs ");
                }

                description.Append($"{curPlayer.PlayerName}");

                appendVs = true;
            }

            return description.ToString();
        }

        public void Clear()
        {
            this.descriptionText.text = string.Empty;
            this.completedSymbol.gameObject.SetActive(false);
        }
    }
}
