namespace Cascadia.Unity.UI.GameScene
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using UnityEngine;

    public class CurrentTurnPanel : MonoBehaviour
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private TurnOrderHolder turnOrderHolder;
        [SerializeReference]
        private HintController hintController;
        [SerializeReference]
        private GameConductor gameConductor;

        [Header("Internal Components")]
        [SerializeReference]
        private Transform currentTurnPanelEntryParent;

        [Header("Prefabs")]
        [SerializeReference]
        private CurrentTurnPanelEntry currentTurnPanelEntryPF;

        public IReadOnlyList<CurrentTurnPanelEntry> Entries => this.entries;
        private List<CurrentTurnPanelEntry> entries { get; set; } = new List<CurrentTurnPanelEntry>();


        public void ResetGame()
        {
            for (int ii = this.currentTurnPanelEntryParent.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.currentTurnPanelEntryParent.GetChild(ii).gameObject);
            }
            this.entries.Clear();

            for (int playerIndex = 0, count = this.turnOrderHolder.PlayerCount; playerIndex < this.turnOrderHolder.PlayerCount; playerIndex++)
            {
                CurrentTurnPanelEntry newTurn = Instantiate(this.currentTurnPanelEntryPF, this.currentTurnPanelEntryParent);
                this.entries.Add(newTurn);

                PlayerSide side = this.gameConductor.CurrentBoardState.PlayerOrder[playerIndex];
                newTurn.RepresentProfile(PersistentGameConfiguration.Singleton.GetPlayerProfile(side), side);

                newTurn.OnShowHints += this.SetHints;
                newTurn.OnHideHints += this.HideHints;
            }

            this.UpdateToNewTurn(this.gameConductor.CurrentBoardState.CurrentPlayerSide);
        }

        public void UpdateToNewTurn(PlayerSide newTurn)
        {
            for (int ii = 0; ii < this.gameConductor.CurrentBoardState.BasedOnConfiguration.PlayerCount; ii++)
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

        public void RestorePlayer(PlayerSide side) => this.entries[side.TurnOrderIndex].RestorePlayer(this.gameConductor.CurrentBoardState.CurrentPlayerSide);

        public void SetCascade(PlayerSide playerSide, int cascadeLevel) => this.entries[playerSide.TurnOrderIndex].SetCascade(cascadeLevel);

        public void SetHints(PlayerSide side)
        {
            for (int ii = 0; ii < this.gameConductor.CurrentBoardState.BasedOnConfiguration.PlayerCount; ii++)
            {
                this.SetHint(ii, ii == side.TurnOrderIndex);
            }
        }

        public void SetHint(int playerIndex, bool shown)
        {
            if (!shown)
            {
                return;
            }

            PlayerSide side = this.gameConductor.CurrentBoardState.PlayerOrder[playerIndex];
            this.hintController.ShowHints(side);
        }

        public void HideHints() => this.hintController.HideHints();
    }
}
