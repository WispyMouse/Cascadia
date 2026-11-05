namespace Cascadia.Unity.UI.Reusable
{
    using Cascadia.DataStructures;
    using Cascadia.Unity;
    using TMPro;
    using UnityEngine;

    public class Cell : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private SpriteRenderer cellSpriteRenderer;
        [SerializeReference]
        private SpriteRenderer iconSpriteRenderer;
        [SerializeReference]
        private Sprite defaultBorder;
        [SerializeReference]
        private Sprite recentBorder;
        [SerializeReference]
        private GameObject hintHolder;
        [SerializeReference]
        private TMP_Text hintNumber;

        public PlayerProfile ControlledProfile { get; private set; }
        public bool AlreadyPlaced => this.SideIndex.HasValue;
        public int? SideIndex => this.ControlledProfile?.Index;
        public Coordinate Position { get; set; }

        private void Awake()
        {
            this.SetHighlightStatus(false);
            this.hintHolder.SetActive(false);
        }

        public void SetSide(PlayerProfile player)
        {
            this.ControlledProfile = player;
            this.iconSpriteRenderer.sprite = player.Sprite.Sprite;
        }

        public void SetHighlightStatus(bool toHighlight) => this.cellSpriteRenderer.sprite = toHighlight ? this.recentBorder : this.defaultBorder;

        public void SetHint(int amount)
        {
            if (amount == 0)
            {
                this.hintHolder.SetActive(false);
            }
            else
            {
                this.hintHolder.SetActive(true);
                this.hintNumber.text = amount.ToString();
            }
        }

        public void Clear()
        {
            this.ControlledProfile = null;
            this.iconSpriteRenderer.sprite = null;
            this.SetHighlightStatus(false);
        }
    }
}
