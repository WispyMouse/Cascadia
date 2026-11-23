namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class WinnerBannerIconShower : MonoBehaviour
    {
        [SerializeReference]
        private Transform holder;

        [SerializeReference]
        private Image leftWinner;

        [SerializeReference]
        private Image rightWinner;

        public void ShowIcons(PlayerProfile winner)
        {
            if (winner.HasSprite)
            {
                this.holder.gameObject.SetActive(true);
                this.leftWinner.sprite = winner.Sprite.Value.Sprite;
                this.rightWinner.sprite = winner.Sprite.Value.Sprite;
            }
            else
            {
                void SetWinnerIcons(PlayerProfile profile)
                {
                    // Ensure that this GameObject isn't null-ish
                    // Check this in case we've navigated scenes before the profile sprite was done
                    if (this == null)
                    {
                        return;
                    }

                    this.holder.gameObject.SetActive(true);
                    this.leftWinner.sprite = profile.Sprite.Value.Sprite;
                    this.rightWinner.sprite = profile.Sprite.Value.Sprite;
                }

                winner.OnPlayerProfileSpriteLoaded += SetWinnerIcons;
            }
        }

        public void HideIcons()
        {
            this.holder.gameObject.SetActive(false);
        }
    }
}
