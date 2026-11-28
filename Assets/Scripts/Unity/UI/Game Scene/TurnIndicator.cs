namespace Cascadia.Unity.UI.GameScene
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TurnIndicator : MonoBehaviour
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private GameConductor gameConductor;

        [Header("Internal Components")]
        [SerializeReference]
        private TurnOrderHolder turnOrderHolder;
        [SerializeReference]
        private TMP_Text turnOrderText;
        [SerializeReference]
        private Image currentTurnImage;

        private void Awake()
        {
            this.turnOrderHolder.OnTurnStarted += this.OnPlayerTurn;
        }

        private void OnPlayerTurn(PlayerSide playerSide, int roundNumber)
        {
            this.turnOrderText.text = $"Round {roundNumber}";

            PlayerProfile profile = PersistentGameConfiguration.Singleton.GetPlayerProfile(playerSide);
            this.currentTurnImage.sprite = null;
            profile.AssignSpriteWhenReady(this.currentTurnImage);
        }
    }
}
