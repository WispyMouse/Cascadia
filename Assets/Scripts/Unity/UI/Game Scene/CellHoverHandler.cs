namespace Cascadia.Unity.UI
{
    using System.Threading.Tasks;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.GameScene;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class CellHoverHandler : MonoBehaviour
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private Camera hoverCamera;
        [SerializeReference]
        private GameConductor gameConductor;
        [SerializeReference]
        private TurnOrderHolder turnOrderHolder;

        [Header("Private Fields")]
        private GameCellRenderer lastHoveredCell { get; set; } = null;

        private async Awaitable Start()
        {
            while (!this.gameConductor.GameConductorWarmedUp)
            {
                await Awaitable.NextFrameAsync();
            }

            // After a move is made, remove the hover and consider re-upping it, in case it's now
            // another player's turn in the right spot
            this.gameConductor.CurrentBoardState.BoardStateEvents.OnPlayerMadeMove += this.ClearAndRetryHover;
        }

        private void Update()
        {
            if (!PersistentGameConfiguration.Singleton.ConfigurationLoaded || this.gameConductor.CurrentBoardState == null)
            {
                return;
            }

            if (this.gameConductor.CurrentBoardState != null && this.gameConductor.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End)
            {
                return;
            }

            if (this.turnOrderHolder.PlayerIsHuman(this.gameConductor.CurrentBoardState.CurrentPlayerSide))
            {
                this.HandleHover();
            }
        }

        private void HandleHover()
        {
            Vector2 raycastPoint;

            if (this.lastHoveredCell != null && !this.lastHoveredCell.AlreadyPlaced)
            {
                this.lastHoveredCell.SetSide(null, GameCellRenderer.ShowOwnershipStateEnum.NotOwned);
            }
            this.lastHoveredCell = null;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                raycastPoint = Touchscreen.current.primaryTouch.position.value;
            }
            else if (Mouse.current != null)
            {
                raycastPoint = Mouse.current.position.value;
            }
            else
            {
                return;
            }

            RaycastHit2D raycast = Physics2D.Raycast(this.hoverCamera.ScreenToWorldPoint(raycastPoint), Vector2.zero);
            if (raycast.collider == null)
            {
                return;
            }

            if (!raycast.collider.TryGetComponent(out GameCellRenderer getCell))
            {
                return;
            }

            if (getCell.AlreadyPlaced)
            {
                return;
            }

            this.HoverCell(getCell);
        }

        private void HoverCell(GameCellRenderer toHover)
        {
            // Debug.Log($"Attempting to hover cell {toHover.Coordinate}", this);
            PlayerSide takingTurn = this.gameConductor.CurrentBoardState.CurrentPlayerSide;
            PlayerProfile player = PersistentGameConfiguration.Singleton.GetPlayerProfile(takingTurn);
            MoveCommand command = new(takingTurn, toHover.Coordinate);

            if (!this.gameConductor.CurrentBoardState.MoveCommandLegalToPlay(command))
            {
                // We probably should not show any errors relating to hovering
                return;
            }

            this.lastHoveredCell = toHover;
            toHover.SetSide(player, GameCellRenderer.ShowOwnershipStateEnum.Previewed);
        }

        private void ClearAndRetryHover(PlayerSide _)
        {
            if (this.lastHoveredCell != null && !this.lastHoveredCell.AlreadyPlaced)
            {
                this.lastHoveredCell.SetSide(null, GameCellRenderer.ShowOwnershipStateEnum.NotOwned);
            }
            this.lastHoveredCell = null;

            this.HandleHover();
        }
    }
}
