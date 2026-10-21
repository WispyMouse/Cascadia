namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class ChooseEmojiCanvas : MonoBehaviour
    {
        [SerializeReference]
        private Transform categoryParent;

        [SerializeReference]
        private Transform optionParent;

        [SerializeReference]
        private ChooseEmojiCategory chooseEmojiCategoryPF;

        [SerializeReference]
        private ChooseEmojiIcon chooseEmojiIconPF;

        private SemaphoreSlim showCategorySemaphore { get; set; } = new SemaphoreSlim(1);

        public PlayerProfile ShowingForProfile { get; private set; }
        private List<ChooseEmojiIcon> icons { get; set; } = new();

        private string selectedCategory { get; set; }
        private ChooseEmojiIcon currentlySelected { get; set; }

        private void Awake()
        {
            this.AdoptEditorBakedIcons();
            this.EstablishCategories();
        }

        public async Awaitable ShowForProfile(PlayerProfile showingForProfile)
        {
            this.gameObject.SetActive(true);
            this.ShowingForProfile = showingForProfile;
            await this.ShowCategory(showingForProfile.Sprite.Category);
        }

        private void EstablishCategories()
        {
            for (int ii = this.categoryParent.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.categoryParent.GetChild(ii).gameObject);
            }

            foreach (string category in SpriteLookup.Singleton.EmojiCategoryLabels)
            {
                ChooseEmojiCategory categoryButton = Instantiate(this.chooseEmojiCategoryPF, this.categoryParent);
                categoryButton.SetCategory(category);
                categoryButton.OnCategoryClicked.AddListener(async (string category) => await this.ShowCategory(category));
            }
        }

        private async Awaitable ShowCategory(string category)
        {
            await this.showCategorySemaphore.WaitAsync();

            for (int ii = this.optionParent.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.optionParent.GetChild(ii).gameObject);
            }

            this.icons.Clear();

            IReadOnlyList<Sprite> sprites = await SpriteLookup.Singleton.GetSpritesFromCategory(category);
            this.icons.Capacity = sprites.Count;

            Debug.Log($"Loaded {sprites.Count} sprites for category {category}");

            int spritesCount = sprites.Count;
            int iconsCount = this.icons.Count;

            // If we need more icons than we currently have already available, make some more
            // (this logic only runs if there are more incoming sprites than already-created icons)
            for (int ii = iconsCount; ii < spritesCount; ii++)
            {
                ChooseEmojiIcon newIcon = Instantiate(this.chooseEmojiIconPF, this.optionParent);
                this.icons.Add(newIcon);
            }

            // Hide the rest
            // (this logic only runs if there are more already-created icons than incoming sprites)
            for (int ii = spritesCount; ii < iconsCount; ii++)
            {
                this.icons[ii].gameObject.SetActive(false);
            }

            for (int ii = 0; ii < spritesCount; ii++)
            {
                Sprite thisSprite = sprites[ii];
                ChooseEmojiIcon icon = this.icons[ii];
                icon.SetSprite(thisSprite);
                icon.gameObject.SetActive(true);

                // If this is the icon the player already has selected, highlight it
                icon.SetSelectionStatus(this.ShowingForProfile.MatchesSprite(thisSprite));
            }

            this.selectedCategory = category;

            if (this.ShowingForProfile.Sprite.Category == this.selectedCategory)
            {
                foreach (ChooseEmojiIcon icon in this.icons)
                {
                    icon.SetSelectionStatus(this.ShowingForProfile.MatchesSprite(icon.RepresentedEmoji));
                }
            }
            else
            {
                foreach (ChooseEmojiIcon icon in this.icons)
                {
                    icon.SetSelectionStatus(false);
                }
            }

            this.showCategorySemaphore.Release();
        }

        public void SelectEmoji(ChooseEmojiIcon selected)
        {
            this.ShowingForProfile.Sprite.Category = this.selectedCategory;
            this.ShowingForProfile.Sprite.Sprite = selected.RepresentedEmoji;
            this.ShowingForProfile.Sprite.SpriteName = selected.RepresentedEmoji.name;

            foreach (ChooseEmojiIcon icon in this.icons)
            {
                icon.SetSelectionStatus(ReferenceEquals(icon, selected));
            }

            if (!ReferenceEquals(this.currentlySelected, null))
            {
                this.currentlySelected.SetSelectionStatus(false);
            }

            this.currentlySelected = selected;
            selected.SetSelectionStatus(true);
        }

        /// <summary>
        /// Creating all of the prefabs for the buttons takes a while.
        /// During editor time, we may have already created some.
        /// Identify the ones currently existing, and "adopt" them into <see cref="icons"/>.
        /// </summary>
        private void AdoptEditorBakedIcons()
        {
            for (int ii = 0, count = this.optionParent.childCount; ii < count; ii++)
            {
                if (this.optionParent.GetChild(ii).TryGetComponent(out ChooseEmojiIcon icon))
                {
                    this.icons.Add(icon);
                    icon.gameObject.SetActive(false);
                }
            }
        }
    }
}
