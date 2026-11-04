namespace Cascadia.Unity.UI.GameScene
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class TurnIndicator : MonoBehaviour
    {
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

            PlayerProfileSprite sprite = PersistentGameConfiguration.Singleton.GetPlayerProfile(playerSide).Sprite;

            if (sprite.NeedsResolution)
            {
                // TODO: Wait for this to load then show it
                this.currentTurnImage.gameObject.SetActive(false);
            }
            else
            {
                this.currentTurnImage.sprite = sprite.Sprite;
                this.currentTurnImage.gameObject.SetActive(true);
            }
        }
    }
}
