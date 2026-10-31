namespace Cascadia.Unity.UI.SetupScene
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class ChooseEmojiCanvas : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Transform categoryParent;
        [SerializeReference]
        private Transform optionParent;
        [SerializeReference]
        private Transform loadingPanel;

        [Header("Prefabs")]
        [SerializeReference]
        private ChooseEmojiCategory chooseEmojiCategoryPF;
        [SerializeReference]
        private ChooseEmojiIcon chooseEmojiIconPF;

        private SemaphoreSlim showCategorySemaphore { get; set; } = new SemaphoreSlim(1);

        public PlayerProfile ShowingForProfile { get; private set; }
        private List<ChooseEmojiIcon> icons { get; set; } = new();

        public string SelectedCategory { get; private set; }
        private ChooseEmojiIcon currentlySelected { get; set; }

        private void Awake()
        {
            this.loadingPanel.gameObject.SetActive(true);
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
                categoryButton.OnCategoryClicked.AddListener((string category) => this.StartShowCategory(category));
            }
        }

        /// <summary>
        /// async void method signature to be compatible with UnityEvents; we intentionally
        /// want to fire-and-forget here without waiting for it to finish
        /// </summary>
        /// <param name="category"></param>
        private async void StartShowCategory(string category)
        {
            // TODO: This should have a cancellation token. At the moment, it'll still finish loading all of the category
            // that's wasted work!
            await this.ShowCategory(category);
        }

        private async Task ShowCategory(string category)
        {
            await this.showCategorySemaphore.WaitAsync();

            this.loadingPanel.gameObject.SetActive(true);

            this.SelectedCategory = category;

            IReadOnlyList<Sprite> sprites = await SpriteLookup.Singleton.GetSpritesFromCategory(category);

            if (this.icons.Capacity < sprites.Count)
            {
                this.icons.Capacity = sprites.Count;
            }

            Debug.Log($"Loaded {sprites.Count} sprites for category {category}", this);

            int spritesCount = sprites.Count;
            int iconsCount = this.icons.Count;

            // If we need more icons than we currently have already available, make some more
            // (this logic only runs if there are more incoming sprites than already-created icons)
            for (int ii = iconsCount; ii < spritesCount; ii++)
            {
                ChooseEmojiIcon newIcon = Instantiate(this.chooseEmojiIconPF, this.optionParent);
                this.icons.Add(newIcon);

                // TODO HACK: Icons need to have this listener added when they're new or previously baked
                // We should have a single place where this listener is added, so that we don't end up
                // with two different hacks of emoji buttons on accident
                newIcon.OnEmojiClicked.AddListener(this.SelectEmoji);
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

            if (this.ShowingForProfile.Sprite.Category == this.SelectedCategory)
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

            this.loadingPanel.gameObject.SetActive(false);

            this.showCategorySemaphore.Release();
        }

        public void SelectEmoji(ChooseEmojiIcon selected)
        {
            this.ShowingForProfile.Sprite.Category = this.SelectedCategory;
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
                    icon.OnEmojiClicked.AddListener(this.SelectEmoji);
                }
            }
        }
    }
}
