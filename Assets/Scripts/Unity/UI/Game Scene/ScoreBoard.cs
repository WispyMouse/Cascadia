namespace Cascadia.Unity.UI.GameScene
{
    using System.Threading.Tasks;
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

            if (winner.HasSprite)
            {
                Image newImage = Instantiate(this.WinnerIconImage, this.WinnerIconHolder);
                newImage.sprite = winner.Sprite.Value.Sprite;
            }
            else
            {
                if (!winner.HasSprite)
                {
                    void SetWinnerSprite(PlayerProfile profile)
                    {
                        // Ensure that this GameObject isn't null-ish
                        // Check this in case we've navigated scenes before the profile sprite was done
                        if (this == null)
                        {
                            return;
                        }

                        Image newImage = Instantiate(this.WinnerIconImage, this.WinnerIconHolder);
                        newImage.sprite = profile.Sprite.Value.Sprite;
                    }

                    winner.OnPlayerProfileSpriteLoaded += SetWinnerSprite;
                }
            }
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
