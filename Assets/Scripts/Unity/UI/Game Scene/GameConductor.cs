namespace Cascadia.Unity.UI.GameScene
{
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.Audio;
    using Cascadia.Unity.UI.Reusable;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class GameConductor : MonoBehaviour
    {
        [SerializeReference]
        private Camera clickCamera;

        [SerializeReference]
        private TurnOrderHolder turnOrderHolder;
        [SerializeReference]
        private GridPainter gridPainter;
        [SerializeReference]
        private CurrentTurnPanel currentTurnPanel;
        [SerializeReference]
        private ScoreBoard scoreBoard;
        [SerializeReference]
        private TMP_Text winnerPanel;
        [SerializeReference]
        private TMP_Text winnerText;
        [SerializeReference]
        private WinnerAnimation winnerAnimation;
        [SerializeReference]
        private Image winnerIcon;
        [SerializeReference]
        private GameObject noMoreMovesPanel;
        [SerializeReference]
        private CascadeBanner cascadeBanner;
        [SerializeReference]
        private StallBanner stallBanner;
        [SerializeReference]
        private WinnerBannerIconShower winnerBannerIconShower;
        [SerializeReference]
        public GameObject rewindButtonHolder;

        public Dictionary<Coordinate, Cell> PositionsToCells { get; set; } = new Dictionary<Coordinate, Cell>();

        [SerializeReference]
        private LineRenderer lineRendererPF;

        private List<LineRenderer> solutionLineRenderers { get; set; } = new List<LineRenderer>();
        private Dictionary<CellsConnection, LineRenderer> connectionToRenderer { get; set; } = new Dictionary<CellsConnection, LineRenderer>();

        public BoardState CurrentBoardState { get; set; }

        public async Awaitable Start()
        {
            while (!PersistentGameConfiguration.Singleton.ConfigurationLoaded)
            {
                await Awaitable.NextFrameAsync();
            }

            this.ResetGame();
        }

        public void ResetGame()
        {
            for (int ii = this.solutionLineRenderers.Count - 1; ii >= 0; ii--)
            {
                Destroy(this.solutionLineRenderers[ii].gameObject);
            }

            // TODO: This is a LOT of UI code tied in directly with the point of user input!
            // We should get as many of these to be off of the GameConductor as possible, and instead
            // THEY subscribe to THIS. As is, every new module is being added *here*
            // Instead, we should fire an event that indicates the show has started, or something
            this.solutionLineRenderers.Clear();
            this.connectionToRenderer.Clear();

            this.winnerAnimation.gameObject.SetActive(false);
            this.winnerPanel.transform.parent.gameObject.SetActive(false);
            this.rewindButtonHolder.SetActive(false);
            this.noMoreMovesPanel.SetActive(false);
            this.winnerBannerIconShower.HideIcons();

            this.CurrentBoardState = new BoardState(PersistentGameConfiguration.Singleton.GameConfiguration);
            this.CurrentBoardState.BoardStateEvents.OnGameConclusion += this.DeclarePlayerVictorious;
            this.CurrentBoardState.BoardStateEvents.OnGameConclusion += this.stallBanner.Clear;
            this.CurrentBoardState.BoardStateEvents.OnPlayerStartTurn += this.currentTurnPanel.UpdateToNewTurn;
            this.CurrentBoardState.BoardStateEvents.OnPlayerRestored += this.currentTurnPanel.RestorePlayer;

            this.PositionsToCells = this.gridPainter.Paint();
            this.currentTurnPanel.ResetGame();
            this.cascadeBanner.ResetGame();

            this.turnOrderHolder.OnTurnStarted?.Invoke(this.CurrentBoardState.CurrentPlayerSide, this.CurrentBoardState.CurrentRound);
        }

        public void Update()
        {
            if (!PersistentGameConfiguration.Singleton.ConfigurationLoaded || this.CurrentBoardState == null)
            {
                return;
            }

            if (this.CurrentBoardState != null && this.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End)
            {
                return;
            }

            if (this.turnOrderHolder.PlayerIsHuman(this.CurrentBoardState.CurrentPlayerSide))
            {
                this.HandleLeftClick();
            }
        }

        public void ChooseCell(Coordinate toChoose) => this.ChooseCell(this.PositionsToCells[toChoose]);

        public void ChooseCell(Cell toChoose)
        {
            Debug.Log($"Attempting to choose cell {toChoose.Position}", this);

            PlayerSide takingTurn = this.CurrentBoardState.CurrentPlayerSide;
            PlayerProfile player = PersistentGameConfiguration.Singleton.GetPlayerProfile(takingTurn);
            MoveCommand command = new(takingTurn, toChoose.Position);

            if (!this.CurrentBoardState.MoveCommandLegalToPlay(command))
            {
                Debug.LogError("Can't play that command! We should tell the user why somehow.", this);
                this.stallBanner.CallAttention();
                return;
            }

            foreach (Cell curCell in this.PositionsToCells.Values)
            {
                curCell.SetHighlightStatus(false);
            }

            RandomBlipPlayer.Singleton.PlayRandomSound();

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

            this.currentTurnPanel.SetCascade(takingTurn, consequences.ConnectionsMade.Count);

            foreach (PlayerSide playerSideRemoved in consequences.PlayerSidesRemoved)
            {
                this.currentTurnPanel.KnockOutPlayer(playerSideRemoved);
            }

            this.CurrentBoardState.ApplyMoveCommand(command);
            this.rewindButtonHolder.SetActive(true);

            toChoose.SetHighlightStatus(true);

            this.turnOrderHolder.OnTurnStarted?.Invoke(this.CurrentBoardState.CurrentPlayerSide, this.CurrentBoardState.CurrentRound);
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

            RaycastHit2D raycast = Physics2D.Raycast(this.clickCamera.ScreenToWorldPoint(raycastPoint), Vector2.zero);
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
            LineRenderer newRenderer = Instantiate(this.lineRendererPF);
            newRenderer.SetPosition(0, this.PositionsToCells[connection.Root].transform.position + (Vector3.back * 5f));
            newRenderer.SetPosition(1, this.PositionsToCells[connection.Tail].transform.position + (Vector3.back * 5f));
            newRenderer.startColor = faction.KnockoutColor;
            newRenderer.endColor = faction.KnockoutColor;
            this.solutionLineRenderers.Add(newRenderer);
            this.connectionToRenderer.Add(connection, newRenderer);
        }

        public void DeclarePlayerVictorious(PlayerSide winner)
        {
            if (winner == null)
            {
                this.winnerAnimation.ToDisplay = null;
                this.winnerPanel.gameObject.SetActive(false);
                this.winnerText.gameObject.SetActive(false);
                this.winnerBannerIconShower.HideIcons();

                // If there are no more moves possible, the game is over! Tough luck, everyone.
                this.noMoreMovesPanel.SetActive(true);
                return;
            }

            PlayerProfile player = PersistentGameConfiguration.Singleton.GetPlayerProfile(winner);

            // this.WinnerPanel.transform.parent.gameObject.SetActive(true);
            this.winnerIcon.sprite = player.Sprite.Sprite;

            string winnerName = player.PlayerName;
            this.winnerAnimation.ToDisplay = player;
            this.winnerAnimation.gameObject.SetActive(true);
            this.winnerPanel.gameObject.SetActive(true);
            this.winnerText.gameObject.SetActive(true);
            this.winnerText.text = winnerName;

            this.winnerBannerIconShower.ShowIcons(player);

            this.scoreBoard.AddToScoreboard(player);
        }

        public void MainMenu() => SceneManager.LoadScene(0);

        /// <summary>
        /// You can't have [int] return type for OnClick functions, so this
        /// simply wraps up <see cref="Rewind"/> in a callable.
        /// </summary>
        public void RewindButton()
        {
            this.Rewind();
        }

        public int Rewind()
        {
            int turnsRewound = 0;

            // If this undid a win, we should remove the "recent wint" entry because this game has been undone
            if (this.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End && this.CurrentBoardState.Winner != null)
            {
                this.winnerBannerIconShower.HideIcons();
                this.scoreBoard.RemoveRecentWin();
            }

            // Rewind until we reach the last human move, or the beginning of the game.
            while (this.CurrentBoardState.MoveCommandsApplied.Count > 0)
            {
                MoveCommandConsequences undoneCommand = this.CurrentBoardState.ReversePreviousMoveCommand();
                turnsRewound++;

                foreach (CellsConnection connectionToRemove in undoneCommand.ConnectionsMade)
                {
                    LineRenderer renderer = this.connectionToRenderer[connectionToRemove];
                    this.connectionToRenderer.Remove(connectionToRemove);
                    this.solutionLineRenderers.Remove(renderer);
                    Destroy(renderer.gameObject);
                }

                foreach (PlayerSide side in undoneCommand.PlayerSidesRemoved)
                {
                    this.currentTurnPanel.RestorePlayer(side);
                }

                this.PositionsToCells[undoneCommand.FromCommand.Position].Clear();

                this.currentTurnPanel.SetCascade(undoneCommand.FromCommand.Side, undoneCommand.PreviousCascadeLevel);

                // When we reach a non-ai human player, stop
                // This way we'll "undo" AI turns until we paint over them.
                // If we don't, our undo might get re-done by the AI
                if (!PersistentGameConfiguration.Singleton.GetPlayerProfile(this.CurrentBoardState.CurrentPlayerSide).IsAI)
                {
                    break;
                }
            }

            // Can't have a winner anymore; the game is no longer over, if it was
            this.noMoreMovesPanel.SetActive(false);
            this.winnerAnimation.gameObject.SetActive(false);
            this.winnerPanel.transform.parent.gameObject.SetActive(false);

            foreach (Cell curCell in this.PositionsToCells.Values)
            {
                curCell.SetHighlightStatus(false);
            }

            // If there is a previous move, then highlight spaces
            // as though this was the most recently made move
            MoveCommandConsequences? latestCommand = this.CurrentBoardState.LastPlay;
            if (latestCommand.HasValue)
            {
                this.PositionsToCells[latestCommand.Value.FromCommand.Position].SetHighlightStatus(true);

                foreach (Coordinate curCoordinate in latestCommand.Value.ConnectionsMade.SelectMany(x => x.Cells))
                {
                    this.PositionsToCells[curCoordinate].SetHighlightStatus(true);
                }
            }

            // Remind it that it's now someone's turn, who might have been eliminated before
            return turnsRewound;
        }
    }
}
