namespace Cascadia.Unity.UI.Reusable
{
    using Cascadia.Unity;
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;

    public class GameCellRenderer : MonoBehaviour, ICellRenderer
    {
        public enum ShowOwnershipStateEnum
        {
            NotOwned,
            Previewed,
            Owned
        }

        [Header("Internal Components")]
        [SerializeReference]
        private SpriteRenderer cellSpriteRenderer;
        [SerializeReference]
        private SpriteRenderer iconSpriteRenderer;
        [SerializeReference]
        private SpriteRenderer previewedSpriteRenderer;
        [SerializeReference]
        private Sprite defaultBorder;
        [SerializeReference]
        private Sprite recentBorder;
        [SerializeReference]
        private GameObject hintHolder;
        [SerializeReference]
        private TMP_Text hintNumber;

        [Header("Public Properties")]
        public GameObject SelfGameObject => this.gameObject;
        public PlayerProfile DisplayedProfile { get; private set; }
        public bool AlreadyPlaced => this.CurrentOwnershipState == ShowOwnershipStateEnum.Owned && this.SideIndex.HasValue;
        public int? SideIndex => this.DisplayedProfile?.Index;
        public Coordinate Coordinate { get; private set; }
        public ShowOwnershipStateEnum CurrentOwnershipState { get; private set; }

        private void Awake()
        {
            this.SetHighlightStatus(false);
            this.hintHolder.SetActive(false);
        }

        public void SetSide(PlayerProfile player, ShowOwnershipStateEnum newState)
        {
            // If for some reason we're setting this to NotOwned through this function,
            // unset the side index and player profile
            if (newState == ShowOwnershipStateEnum.NotOwned)
            {
                this.Clear();
                return;
            }

            this.DisplayedProfile = player;

            SpriteRenderer rendererToSelect = newState == ShowOwnershipStateEnum.Previewed ? this.previewedSpriteRenderer : this.iconSpriteRenderer;
            SpriteRenderer rendererToDeSelect = newState == ShowOwnershipStateEnum.Previewed ? this.iconSpriteRenderer : this.previewedSpriteRenderer;

            rendererToDeSelect.sprite = null;

            if (player.HasSprite)
            {
                rendererToSelect.sprite = player.Sprite.Value.Sprite;
            }
            else
            {
                rendererToSelect.sprite = null;

                void UpdateSpriteFromProfile(PlayerProfile loaded)
                {
                    // Ensure that this GameObject isn't null-ish
                    // Check this in case we've navigated scenes before the profile sprite was done
                    if (this == null)
                    {
                        return;
                    }

                    rendererToSelect.sprite = loaded.Sprite.Value.Sprite;
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
            this.DisplayedProfile = null;
            this.iconSpriteRenderer.sprite = null;
            this.previewedSpriteRenderer.sprite = null;
            this.SetHighlightStatus(false);
        }

        public void SetLocalPosition(Vector2 position, Coordinate coordinate)
        {
            this.transform.localPosition = position;
            this.Coordinate = coordinate;
        }
    }
}
