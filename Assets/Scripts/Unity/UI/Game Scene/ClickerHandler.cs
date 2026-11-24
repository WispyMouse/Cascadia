namespace Cascadia.Unity.UI
{
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.GameScene;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class ClickerHandler : MonoBehaviour
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private Camera clickCamera;
        [SerializeReference]
        private GameConductor gameConductor;
        [SerializeReference]
        private TurnOrderHolder turnOrderHolder;

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
                this.HandleLeftClick();
            }
        }

        private void HandleLeftClick()
        {
            Vector2 raycastPoint;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                raycastPoint = Touchscreen.current.primaryTouch.position.value;
            }
            else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                raycastPoint = Mouse.current.position.value;
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

            if (!raycast.collider.TryGetComponent(out GameCellRenderer getCell))
            {
                return;
            }

            if (getCell.AlreadyPlaced)
            {
                return;
            }

            this.gameConductor.ChooseCell(getCell);
        }
    }
}
