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
            this.holder.gameObject.SetActive(true);
            this.leftWinner.sprite = winner.Sprite.Sprite;
            this.rightWinner.sprite = winner.Sprite.Sprite;
        }

        public void HideIcons()
        {
            this.holder.gameObject.SetActive(false);
        }
    }
}
