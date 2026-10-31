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
            // TODO: If the sprite hasn't loaded for some reason (is this a unit test?) we don't want to null ref
            // what should we do?
            if (!winner.Sprite.NeedsResolution)
            {
                this.holder.gameObject.SetActive(true);
                this.leftWinner.sprite = winner.Sprite.Sprite;
                this.rightWinner.sprite = winner.Sprite.Sprite;
            }
            else
            {
                Debug.Log($"Attempted to show winner banner icons, but the sprite hasn't resolved yet.", this);
            }
        }

        public void HideIcons()
        {
            this.holder.gameObject.SetActive(false);
        }
    }
}
