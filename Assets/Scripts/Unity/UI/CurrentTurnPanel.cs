namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using UnityEngine;

    public class CurrentTurnPanel : MonoBehaviour
    {
        public CurrentTurnPanelEntry CurrentTurnPanelEntryPF;
        public Transform CurrentTurnPanelEntryParent;
        private List<CurrentTurnPanelEntry> entries { get; set; } = new List<CurrentTurnPanelEntry>();

        public TurnOrderHolder TurnOrderHolder;
        public HintController HintController;

        public void ResetGame()
        {
            for (int ii = this.CurrentTurnPanelEntryParent.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.CurrentTurnPanelEntryParent.GetChild(ii).gameObject);
            }
            this.entries.Clear();

            for (int playerIndex = 0, count = this.TurnOrderHolder.PlayerCount; playerIndex < this.TurnOrderHolder.PlayerCount; playerIndex++)
            {
                CurrentTurnPanelEntry newTurn = Instantiate(this.CurrentTurnPanelEntryPF, this.CurrentTurnPanelEntryParent);
                this.entries.Add(newTurn);

                PlayerSide side = this.TurnOrderHolder.GameConductor.CurrentBoardState.PlayerOrder[playerIndex];
                newTurn.RepresentProfile(PersistentGameConfiguration.Singleton.GetPlayerProfile(side));

                newTurn.OnShowHints += this.SetHints;
                newTurn.OnHideHints += this.HideHints;
            }

            this.UpdateToNewTurn(this.TurnOrderHolder.GameConductor.CurrentBoardState.CurrentPlayerSide);
        }

        public void UpdateToNewTurn(PlayerSide newTurn)
        {
            for (int ii = 0; ii < this.TurnOrderHolder.GameConductor.CurrentBoardState.BasedOnConfiguration.PlayerCount; ii++)
            {
                this.entries[ii].SetCurrentTurn(ii == newTurn.TurnOrderIndex);
            }

            this.HideHints();
        }

        public void KnockOutPlayer(PlayerSide playerSide)
        {
            // We're assuming that entries are arranged in turn order index, so we can knockout
            // whatever entry is mapped to that turn order
            this.entries[playerSide.TurnOrderIndex].MarkKnockout();
        }

        public void RestorePlayer(PlayerSide side) => this.entries[side.TurnOrderIndex].RestorePlayer(this.TurnOrderHolder.GameConductor.CurrentBoardState.CurrentPlayerSide);

        public void SetCascade(int index, int cascadeLevel) => this.entries[index].SetCascade(cascadeLevel);

        public void SetHints(int visibleHint)
        {
            for (int ii = 0; ii < this.TurnOrderHolder.GameConductor.CurrentBoardState.BasedOnConfiguration.PlayerCount; ii++)
            {
                this.SetHint(ii, ii == visibleHint);
            }
        }

        public void SetHint(int playerIndex, bool shown)
        {
            if (!shown)
            {
                return;
            }

            PlayerSide side = this.TurnOrderHolder.GameConductor.CurrentBoardState.PlayerOrder[playerIndex];
            this.HintController.ShowHints(side);
        }

        public void HideHints() => this.HintController.HideHints();
    }
}
