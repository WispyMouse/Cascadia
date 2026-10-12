namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ScoreBoard : MonoBehaviour
    {
        public Image WinnerIconImage;

        public TurnOrderHolder TurnOrderHolder;

        public Transform WinnerIconHolder;

        private int maxDisplayedWins { get; set; } = 9;

        private void Awake()
        {
            for (int ii = this.WinnerIconHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.WinnerIconHolder.GetChild(ii).gameObject);
            }
        }

        public void AddToScoreboard(PlayerProfile winner)
        {
            if (this.WinnerIconHolder.childCount >= this.maxDisplayedWins)
            {
                Destroy(this.WinnerIconHolder.GetChild(0).gameObject);
            }

            Sprite winnerSprite = winner.SpriteRepresentation;
            Image newImage = Instantiate(this.WinnerIconImage, this.WinnerIconHolder);
            newImage.sprite = winnerSprite;
        }

        public void RemoveRecentWin()
        {
            int childCount = this.WinnerIconHolder.childCount;

            if (childCount == 0)
            {
                return;
            }

            Destroy(this.WinnerIconHolder.GetChild(childCount - 1).gameObject);
        }
    }
}
