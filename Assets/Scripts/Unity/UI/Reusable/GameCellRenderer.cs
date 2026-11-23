namespace Cascadia.Unity.UI.Reusable
{
    using Cascadia.Unity;
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;

    public class GameCellRenderer : MonoBehaviour, ICellRenderer
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
        public Coordinate Coordinate { get; private set; }

        public GameObject SelfGameObject => this.gameObject;

        private void Awake()
        {
            this.SetHighlightStatus(false);
            this.hintHolder.SetActive(false);
        }

        public void SetSide(PlayerProfile player)
        {
            this.ControlledProfile = player;

            if (player.HasSprite)
            {
                this.iconSpriteRenderer.sprite = player.Sprite.Value.Sprite;
            }
            else
            {
                void UpdateSpriteFromProfile(PlayerProfile loaded)
                {
                    // Ensure that this GameObject isn't null-ish
                    // Check this in case we've navigated scenes before the profile sprite was done
                    if (this == null)
                    {
                        return;
                    }

                    this.iconSpriteRenderer.sprite = loaded.Sprite.Value.Sprite;
                }
                player.OnPlayerProfileSpriteLoaded += UpdateSpriteFromProfile;
            }
        }

        public void SetHighlightStatus(bool toHighlight)
        {
            this.cellSpriteRenderer.sprite = toHighlight ? this.recentBorder : this.defaultBorder;
        }

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

        public void SetLocalPosition(Vector2 position, Coordinate coordinate)
        {
            this.transform.localPosition = position;
            this.Coordinate = coordinate;
        }
    }
}
