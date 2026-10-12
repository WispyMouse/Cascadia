namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class GameConductor : MonoBehaviour
    {
        public Camera ClickCamera;

        public TurnOrderHolder TurnOrderHolder;
        public GridPainter GridPainter;
        public CurrentTurnPanel CurrentTurnPanel;
        public ScoreBoard ScoreBoard;
        public TMP_Text WinnerPanel;
        public TMP_Text WinnerText;
        public Image WinnerIcon;
        public GameObject NoMoreMovesPanel;
        public CascadeBanner CascadeBanner;
        public StallBanner StallBanner;

        public Dictionary<Coordinate, Cell> PositionsToCells { get; set; } = new Dictionary<Coordinate, Cell>();

        public LineRenderer LineRendererPF;
        private List<LineRenderer> solutionLineRenderers { get; set; } = new List<LineRenderer>();
        private Dictionary<CellsConnection, LineRenderer> connectionToRenderer { get; set; } = new Dictionary<CellsConnection, LineRenderer>();

        public BoardState CurrentBoardState { get; set; }

        public GameObject RewindButtonHolder;

        public void Start() => this.ResetGame();

        public void ResetGame()
        {
            for (int ii = this.solutionLineRenderers.Count - 1; ii >= 0; ii--)
            {
                Destroy(this.solutionLineRenderers[ii].gameObject);
            }
            this.solutionLineRenderers.Clear();
            this.connectionToRenderer.Clear();

            this.WinnerPanel.transform.parent.gameObject.SetActive(false);
            this.RewindButtonHolder.SetActive(false);
            this.NoMoreMovesPanel.SetActive(false);

            this.CurrentBoardState = new BoardState(PersistentGameConfiguration.Singleton.GameConfiguration);
            this.CurrentBoardState.OnGameConclusion += this.DeclarePlayerVictorious;
            this.CurrentBoardState.OnGameConclusion += this.StallBanner.Clear;
            this.CurrentBoardState.OnPlayerStartTurn += this.CurrentTurnPanel.UpdateToNewTurn;

            this.PositionsToCells = this.GridPainter.Paint();
            this.CurrentTurnPanel.ResetGame();
            this.CascadeBanner.ResetGame();

            this.TurnOrderHolder.OnTurnStarted?.Invoke(this.CurrentBoardState.CurrentPlayerSide);
        }

        public void Update()
        {
            if (this.CurrentBoardState != null && this.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End)
            {
                return;
            }

            if (this.TurnOrderHolder.PlayerIsHuman(this.CurrentBoardState.CurrentPlayerSide))
            {
                this.HandleLeftClick();
            }
        }

        public void ChooseCell(Coordinate toChoose) => this.ChooseCell(this.PositionsToCells[toChoose]);

        public void ChooseCell(Cell toChoose)
        {
            foreach (Cell curCell in this.PositionsToCells.Values)
            {
                curCell.SetHighlightStatus(false);
            }

            RandomBlipPlayer.Singleton.PlayRandomSound();

            int takingTurn = this.CurrentBoardState.CurrentPlayerSide;
            PlayerProfile player = PersistentGameConfiguration.Singleton.Players[takingTurn];
            MoveCommand command = new(takingTurn, toChoose.Position);

            if (!this.CurrentBoardState.MoveCommandLegalToPlay(command))
            {
                Debug.Log("Can't play that command! We should tell the user why somehow.");
                this.StallBanner.CallAttention();
                return;
            }

            MoveCommandConsequences consequences = this.CurrentBoardState.GenerateConsequencesFromMoveCommand(command);

            toChoose.SetSide(player);

            foreach (CellsConnection solution in consequences.ConnectionsMade)
            {
                this.DrawLineBetween(solution, player);

                foreach (Coordinate curCell in solution.Cells)
                {
                    this.PositionsToCells[curCell].SetHighlightStatus(true);
                }
            }

            this.CurrentTurnPanel.SetCascade(takingTurn, consequences.ConnectionsMade.Count);

            foreach (int sideRemoved in consequences.PlayersSidesRemoved)
            {
                this.CurrentTurnPanel.KnockOutPlayer(sideRemoved);
            }

            this.CurrentBoardState.TryApplyMoveCommand(command);
            this.RewindButtonHolder.SetActive(true);

            toChoose.SetHighlightStatus(true);
        }

        private void HandleLeftClick()
        {
            Vector2 raycastPoint;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                raycastPoint = Mouse.current.position.value;
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                raycastPoint = Touchscreen.current.primaryTouch.position.value;
            }
            else
            {
                return;
            }

            RaycastHit2D raycast = Physics2D.Raycast(this.ClickCamera.ScreenToWorldPoint(raycastPoint), Vector2.zero);
            if (raycast.collider == null)
            {
                return;
            }

            if (!raycast.collider.TryGetComponent(out Cell getCell))
            {
                return;
            }

            if (getCell.AlreadyPlaced)
            {
                return;
            }

            this.ChooseCell(getCell);
        }

        public void DrawLineBetween(CellsConnection connection, PlayerProfile faction)
        {
            LineRenderer newRenderer = Instantiate(this.LineRendererPF);
            newRenderer.SetPosition(0, this.PositionsToCells[connection.Root].transform.position + (Vector3.back * 5f));
            newRenderer.SetPosition(1, this.PositionsToCells[connection.Tail].transform.position + (Vector3.back * 5f));
            newRenderer.startColor = faction.KnockoutColor;
            newRenderer.endColor = faction.KnockoutColor;
            this.solutionLineRenderers.Add(newRenderer);
            this.connectionToRenderer.Add(connection, newRenderer);
        }

        public void DeclarePlayerVictorious(int? winner)
        {
            PlayerProfile player = PersistentGameConfiguration.Singleton.Players[this.CurrentBoardState.CurrentPlayerSide];

            // this.WinnerPanel.transform.parent.gameObject.SetActive(true);
            this.WinnerIcon.sprite = player.SpriteRepresentation;

            string winnerName = player.PlayerName;
            if (!string.IsNullOrEmpty(winnerName))
            {
                this.WinnerText.gameObject.SetActive(true);
                this.WinnerText.text = winnerName;
            }
            else
            {
                this.WinnerText.gameObject.SetActive(false);
            }

            this.ScoreBoard.AddToScoreboard(player);
        }

        public void MainMenu() => SceneManager.LoadScene(0);

        public void Rewind()
        {
            // If this undid a win, we should remove the "recent wint" entry because this game has been undone
            if (this.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End && this.CurrentBoardState.Winner.HasValue)
            {
                this.ScoreBoard.RemoveRecentWin();
            }

            // Rewind until we reach the last human move, or the beginning of the game.
            while (this.CurrentBoardState.MoveCommandsApplied.Count > 0)
            {
                MoveCommandConsequences undoneCommand = this.CurrentBoardState.ReversePreviousMoveCommand();

                foreach (CellsConnection connectionToRemove in undoneCommand.ConnectionsMade)
                {
                    LineRenderer renderer = this.connectionToRenderer[connectionToRemove];
                    this.connectionToRenderer.Remove(connectionToRemove);
                    this.solutionLineRenderers.Remove(renderer);
                    Destroy(renderer.gameObject);
                }

                foreach (int playerRemoved in undoneCommand.PlayersSidesRemoved)
                {
                    this.CurrentTurnPanel.RestorePlayer(playerRemoved);
                }

                this.PositionsToCells[undoneCommand.FromCommand.Position].Clear();

                // When we reach a player, stop
                if (!PersistentGameConfiguration.Singleton.Players[this.CurrentBoardState.CurrentPlayerSide].IsAI)
                {
                    break;
                }
            }

            // We have to have more moves, right?
            // Can't have a winner anymore either
            this.NoMoreMovesPanel.SetActive(false);
            this.WinnerPanel.transform.parent.gameObject.SetActive(false);

            foreach (Cell curCell in this.PositionsToCells.Values)
            {
                curCell.SetHighlightStatus(false);
            }

            if (this.CurrentBoardState.MoveCommandsApplied.Count > 0)
            {
                MoveCommandConsequences latestCommand = this.CurrentBoardState.MoveCommandsApplied[this.CurrentBoardState.MoveCommandsApplied.Count - 1];

                this.PositionsToCells[latestCommand.FromCommand.Position].SetHighlightStatus(true);

                foreach (Coordinate curCoordinate in latestCommand.ConnectionsMade.SelectMany(x => x.Cells))
                {
                    this.PositionsToCells[curCoordinate].SetHighlightStatus(true);
                }
            }

            // Remind it that it's now someone's turn, who might have been eliminated before

        }
    }
}
