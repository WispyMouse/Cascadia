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

            if (profile.HasSprite)
            {
                PlayerProfileSprite sprite = profile.Sprite.Value;
                this.currentTurnImage.sprite = sprite.Sprite;
                this.currentTurnImage.gameObject.SetActive(true);
            }
            else
            {
                void SetPlayerProfileSprite(PlayerProfile sprite)
                {
                    // Ensure that this GameObject isn't null-ish
                    // Check this in case we've navigated scenes before the profile sprite was done
                    if (this == null || this.gameConductor == null)
                    {
                        return;
                    }

                    // Only set the sprite after loading if it is still that player's turn
                    if (this.gameConductor.CurrentBoardState.CurrentPlayerSide != playerSide)
                    {
                        return;
                    }

                    this.currentTurnImage.sprite = sprite.Sprite.Value.Sprite;
                    this.currentTurnImage.gameObject.SetActive(true);
                }
                profile.OnPlayerProfileSpriteLoaded += SetPlayerProfileSprite;
            }
        }
    }
}
