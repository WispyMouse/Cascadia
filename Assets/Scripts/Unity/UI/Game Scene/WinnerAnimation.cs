namespace Cascadia.Unity.UI.GameScene
{
    using UnityEngine;
    using System.Collections.Generic;

    public class WinnerAnimation : MonoBehaviour
    {
        public PlayerProfile ToDisplay;

        private List<WinnerAnimationItem> winnerAnimations { get; set; } = new();
        [SerializeReference]
        private WinnerAnimationItem winnerAnimationItemPF;

        [SerializeField]
        private float distanceOut = 100f;

        [SerializeField]
        private int amount = 0;

        private void OnEnable()
        {
            if (this.ToDisplay == null)
            {
                Debug.LogError($"Set {nameof(this.ToDisplay)} before making enabled in order to display animation.", this);
                return;
            }

            for (int ii = 0; ii < this.amount; ii++)
            {
                WinnerAnimationItem newItem = Instantiate(this.winnerAnimationItemPF, this.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Random.insideUnitCircle * this.distanceOut;

                if (this.ToDisplay.HasSprite)
                {
                    newItem.Image.sprite = this.ToDisplay.Sprite.Value.Sprite;
                }
                else
                {
                    void LoadInSpriteWhenDone(PlayerProfile profile)
                    {
                        // Ensure that this GameObject isn't null-ish
                        // Check this in case we've navigated scenes before the profile sprite was done
                        if (this == null)
                        {
                            return;
                        }

                        newItem.Image.sprite = profile.Sprite.Value.Sprite;
                    }
                    this.ToDisplay.OnPlayerProfileSpriteLoaded += LoadInSpriteWhenDone;
                }

                this.winnerAnimations.Add(newItem);
            }
        }

        private void OnDisable()
        {
            for (int ii = 0; ii < this.winnerAnimations.Count; ii++)
            {
                GameObject.Destroy(this.winnerAnimations[ii].gameObject);
            }
            this.winnerAnimations.Clear();
        }
    }
}
