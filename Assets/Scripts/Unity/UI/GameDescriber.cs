namespace Cascadia.Unity.UI
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
    }
}
