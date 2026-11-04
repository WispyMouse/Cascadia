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

        [Header("Data Resources")]
        [SerializeReference]
        private List<Sprite> clearSprites;

        public void OnEnable() => PersistentGameConfiguration.Singleton.OnGameplaySettingsUpdated += this.UpdateDescription;
        public void OnDisable() => PersistentGameConfiguration.Singleton.OnGameplaySettingsUpdated -= this.UpdateDescription;

        public void UpdateDescriptionFromCurrentConfiguration()
        {
            this.UpdateDescription(PersistentGameConfiguration.Singleton.GameConfiguration);
        }

        public void UpdateDescription(ChallengeDefinition definition, int clearCount)
        {
            StringBuilder description = new();

            description.AppendLine(definition.ChallengeName);
            description.AppendLine($"{definition.AICores.Count + 1} players");
            description.AppendLine($"{definition.Width}x{definition.Height}");

            if (definition.StallTurn > 0)
            {
                description.AppendLine($"No connections until turn {definition.StallTurn}");
            }

            string appendSpace = string.Empty;

            for (int ii = 0; ii < definition.AICores.Count; ii++)
            {
                description.Append($"{appendSpace}vs {definition.AICores[ii].name}");
                appendSpace = " ";
            }

            this.descriptionText.text = description.ToString();

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
            StringBuilder description = new();

            description.AppendLine($"{PersistentGameConfiguration.Singleton.Players.Count} players");
            description.AppendLine($"{PersistentGameConfiguration.Singleton.Width}x{PersistentGameConfiguration.Singleton.Height}");

            if (PersistentGameConfiguration.Singleton.StallTurn > 0)
            {
                description.AppendLine($"No connections until turn {PersistentGameConfiguration.Singleton.StallTurn}");
            }

            bool appendVs = false;
            foreach (PlayerProfile curPlayer in PersistentGameConfiguration.Singleton.Players)
            {
                if (appendVs)
                {
                    description.Append($" vs ");
                }

                description.Append($"{curPlayer.PlayerName}");

                appendVs = true;
            }

            this.descriptionText.text = description.ToString();
        }

        public void Clear()
        {
            this.descriptionText.text = string.Empty;
            this.completedSymbol.gameObject.SetActive(false);
        }
    }
}
