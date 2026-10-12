namespace Cascadia.Unity.UI
{
    using Cascadia.DataStructures;
    using Cascadia.Unity;
    using UnityEngine;

    public class CascadeBanner : MonoBehaviour
    {
        public GameConductor GameConductor;
        public GameObject ToggleParent;
        public CascadeText CascadeAnimator;

        public float NoWinnerSpeed = .7f;
        public float WinnerSpeed = 3f;

        public void ResetGame()
        {
            this.GameConductor.CurrentBoardState.OnPlayerMadeMove += this.OnMoveMade;
            this.GameConductor.CurrentBoardState.OnMoveUndone += this.OnMoveUndone;
            this.GameConductor.CurrentBoardState.OnGameConclusion += this.OnGameConclusion;
            this.ToggleParent.SetActive(false);
        }

        public void OnMoveUndone(MoveCommandConsequences undone) => this.OnMoveMade(this.GameConductor.CurrentBoardState.CurrentPlayerSide);

        public void OnMoveMade(int playerSide)
        {
            if (this.GameConductor.CurrentBoardState.CurrentCascadeLevel == 0)
            {
                this.ToggleParent.SetActive(false);
                return;
            }

            this.ToggleParent.SetActive(true);
            if (this.GameConductor.CurrentBoardState.CurrentCascadeLevel == 1)
            {
                this.CascadeAnimator.TextString = "Row x1";
            }
            else
            {
                this.CascadeAnimator.TextString = $"Cascade x{this.GameConductor.CurrentBoardState.CurrentCascadeLevel}";
            }

            this.CascadeAnimator.EnableAnimation(1f / this.GameConductor.CurrentBoardState.CurrentCascadeLevel);
        }

        public void OnGameConclusion(int? winner)
        {
            if (winner.HasValue)
            {
                this.CascadeAnimator.TextString = $"{PersistentGameConfiguration.Singleton.Players[winner.Value].PlayerName} WINS!";
                this.CascadeAnimator.EnableAnimation(this.WinnerSpeed);
            }
            else
            {
                this.CascadeAnimator.TextString = "~NO WINNER~";
                this.CascadeAnimator.EnableAnimation(this.NoWinnerSpeed);
            }
        }
    }
}
