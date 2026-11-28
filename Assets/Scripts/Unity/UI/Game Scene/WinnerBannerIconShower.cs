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
            this.leftWinner.sprite = null;
            this.rightWinner.sprite = null;
            winner.AssignSpriteWhenReady(this.leftWinner);
            winner.AssignSpriteWhenReady(this.rightWinner);
        }

        public void HideIcons()
        {
            this.holder.gameObject.SetActive(false);
        }
    }
}
