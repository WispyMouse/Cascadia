namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using UnityEngine;

    public class PlayerConfiguratorSelector : MonoBehaviour
    {
        public List<PlayerOperationsButton> PlayerButtons = new();

        private void OnEnable() => this.UpdateFromMinusOrPlus();

        public void UpdateFromMinusOrPlus()
        {
            int playerCount = PersistentGameConfiguration.Singleton.Players.Count;
            for (int ii = 0, count = this.PlayerButtons.Count; ii < count; ii++)
            {
                this.PlayerButtons[ii].MinusButtonBase.SetActive(ii > BoardState.MINIMUMPLAYERS);

                if (playerCount > ii)
                {
                    this.PlayerButtons[ii].SetFromProfile(PersistentGameConfiguration.Singleton.Players[ii]);
                }
                else
                {
                    // Show the + button on the "next" player that should be available
                    this.PlayerButtons[ii].Deactivate(showPlus: ii == playerCount);
                }
            }
        }

        public int CurrentlyConfigurablePlayerButtonsCount
        {
            get
            {
                int playerButtonsVisible = 0;

                for (int ii = 0; ii < this.PlayerButtons.Count; ii++)
                {
                    playerButtonsVisible += this.PlayerButtons[ii].IsShowingConfiguration ? 1 : 0;
                }

                return playerButtonsVisible;
            }
        }
    }
}
