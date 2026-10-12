namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public class HintController : MonoBehaviour
    {
        public GameConductor GameConductor;
        public TurnOrderHolder TurnOrderHolder;
        public GridPainter GridPainter;

        private bool isOn { get; set; } = false;

        public void Start() =>
            // As it becomes a new player's turn, if hints are on, update them
            // It doesn't become the new player's hints, but we may need to update the hints for the displaying player
            this.TurnOrderHolder.OnTurnStarted += this.RefreshHints;

        public void ShowHints(int side)
        {
            foreach (Cell cell in this.GameConductor.PositionsToCells.Values)
            {
                cell.SetHint(0);

                if (cell.AlreadyPlaced)
                {
                    continue;
                }

                cell.SetHint(this.GameConductor.CurrentBoardState.GenerateConsequencesFromMoveCommand(new DataStructures.MoveCommand(side, cell.Position)).ConnectionsMade.Count);
            }
            this.isOn = true;
        }

        public void HideHints()
        {
            foreach (Cell cell in this.GameConductor.PositionsToCells.Values)
            {
                cell.SetHint(0);
            }
            this.isOn = false;
        }

        public void RefreshHints(int side)
        {
            if (!this.isOn)
            {
                return;
            }

            this.ShowHints(side);
        }
    }
}
