namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using static Codice.Client.BaseCommands.Import.Commit;

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

        private void Awake()
        {
            this.EstablishCategories();
        }

        public async Awaitable ShowForProfile(PlayerProfile showingForProfile)
        {
            this.ShowingForProfile = showingForProfile;
            this.gameObject.SetActive(true);
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

            foreach (Sprite sprite in sprites)
            {
                ChooseEmojiIcon iconButton = Instantiate(this.chooseEmojiIconPF, this.optionParent);
                iconButton.SetSprite(sprite);
                iconButton.OnEmojiClicked.AddListener(this.SelectEmoji);
                this.icons.Add(iconButton);
            }

            this.selectedCategory = category;

            foreach (ChooseEmojiIcon icon in this.icons)
            {
                icon.SetSelectionStatus(ReferenceEquals(icon.RepresentedEmoji, this.ShowingForProfile.Sprite));
            }

            this.showCategorySemaphore.Release();
        }

        public void SelectEmoji(ChooseEmojiIcon selected)
        {
            this.ShowingForProfile.Sprite.Category = this.selectedCategory;
            this.ShowingForProfile.Sprite.Sprite = selected.RepresentedEmoji;

            foreach (ChooseEmojiIcon icon in this.icons)
            {
                icon.SetSelectionStatus(ReferenceEquals(icon, selected));
            }
        }
    }
}
