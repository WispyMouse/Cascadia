namespace Cascadia.Unity.UI.SetupScene
{
    using System.Text;
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// Describes the current game configuration and outputs it in the Game Setup menu.
    /// </summary>
    public class GameDescriber : MonoBehaviour
    {
        public TMP_Text DescriptionText;

        public void OnEnable() => PersistentGameConfiguration.Singleton.OnGameplaySettingsUpdated += this.UpdateDescription;
        public void OnDisable() => PersistentGameConfiguration.Singleton.OnGameplaySettingsUpdated -= this.UpdateDescription;

        public void UpdateDescriptionFromCurrentConfiguration()
        {
            this.UpdateDescription(PersistentGameConfiguration.Singleton.GameConfiguration);
        }

        public void UpdateDescription(ChallengeDefinition definition)
        {
            StringBuilder description = new();

            description.AppendLine(definition.ChallengeName);
            description.AppendLine($"{definition.AICores.Count + 1} players");
            description.AppendLine($"{definition.Width}x{definition.Height}");

            if (definition.StallTurn > 0)
            {
                description.AppendLine($"No connections until turn {definition.StallTurn}");
            }

            for (int ii = 0; ii < definition.AICores.Count; ii++)
            {
                description.Append($"vs {definition.AICores[ii].name}");
            }

            this.DescriptionText.text = description.ToString();
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

            this.DescriptionText.text = description.ToString();
        }

        public void Clear()
        {
            this.DescriptionText.text = string.Empty;
        }
    }
}
