namespace TicTacCOSTCO.Unity.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using TicTacCOSTCO.DataStructures;
    using UnityEngine;
    using UnityEngine.Profiling;

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

            for (int playerIndex = 0; playerIndex < this.TurnOrderHolder.PlayerCount; playerIndex++)
            {
                CurrentTurnPanelEntry newTurn = Instantiate(this.CurrentTurnPanelEntryPF, this.CurrentTurnPanelEntryParent);
                this.entries.Add(newTurn);

                int side = this.TurnOrderHolder.GameConductor.CurrentBoardState.PlayerOrder[playerIndex];
                newTurn.RepresentProfile(PersistentGameConfiguration.Singleton.Players[side]);

                newTurn.OnShowHints += SetHints;
                newTurn.OnHideHints += HideHints;
            }

            this.UpdateToNewTurn(this.TurnOrderHolder.GameConductor.CurrentBoardState.CurrentPlayerIndex);
        }

        public void UpdateToNewTurn(int newTurnPlayerIndex)
        {
            for (int ii = 0; ii < this.TurnOrderHolder.GameConductor.CurrentBoardState.PlayerCount; ii++)
            {
                this.entries[ii].SetCurrentTurn(ii == newTurnPlayerIndex);
            }

            this.HideHints();
        }

        public void KnockOutPlayer(int index)
        {
            for (int ii = 0; ii < this.entries.Count; ii++)
            {
                if (this.entries[ii].RepresentedProfile.Index == index)
                {
                    this.entries[ii].MarkKnockout();
                    break;
                }
            }
        }

        public void RestorePlayer(int index)
        {
            this.entries[index].RestorePlayer(this.TurnOrderHolder.GameConductor.CurrentBoardState.CurrentPlayerSide);
        }

        public void SetCascade(int index, int cascadeLevel)
        {
            this.entries[index].SetCascade(cascadeLevel);
        }

        public void SetHints(int visibleHint)
        {
            for (int ii = 0; ii < this.TurnOrderHolder.GameConductor.CurrentBoardState.PlayerCount; ii++)
            {
                SetHint(ii, ii == visibleHint);
            }
        }

        public void SetHint(int playerIndex, bool shown)
        {
            if (!shown)
            {
                return;
            }

            int side = this.TurnOrderHolder.GameConductor.CurrentBoardState.PlayerOrder[playerIndex];
            this.HintController.ShowHints(side);
        }

        public void HideHints()
        {
            this.HintController.HideHints();
        }
    }
}