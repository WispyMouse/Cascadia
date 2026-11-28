namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Cascadia.Unity.UI;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceLocations;
    using UnityEngine.U2D;

    public class SpriteLookup : MonoBehaviour
    {
        public static SpriteLookup Singleton { get; private set; } = null;

        public IReadOnlyList<string> EmojiCategoryLabels => this.emojiCategoryLabels;
        [SerializeField]
        private List<string> emojiCategoryLabels = new();
        private SemaphoreSlim spriteLoadingSemaphore { get; set; } = new SemaphoreSlim(1);

        private void Awake()
        {
            // If we already have an instance, don't make another
            if (Singleton != null)
            {
                Destroy(this.gameObject);
                return;
            }

            // This is now the singleton; unparent it so that we can mark as DontDestroyOnLoad
            Singleton = this;
            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }

        public async Awaitable<IReadOnlyList<Sprite>> GetSpritesFromCategory(string category)
        {
            await this.spriteLoadingSemaphore.WaitAsync();

            Debug.Log($"Category chosen: {category}", this);

            AsyncOperationHandle<IList<SpriteAtlas>> allAtlasWithCategory = Addressables.LoadAssetAsync<IList<SpriteAtlas>>(category);
            await allAtlasWithCategory.Task;

            IList<SpriteAtlas> spriteAtlases = allAtlasWithCategory.Result;

            if (spriteAtlases.Count == 0)
            {
                throw new System.Exception($"Couldn't find any sprite atlas with the category '{category}'.");
            }

            SpriteAtlas randomAtlas = spriteAtlases[Random.Range(0, spriteAtlases.Count)];

            if (randomAtlas.spriteCount == 0)
            {
                throw new System.Exception($"Atlas {randomAtlas} has no sprites.");
            }

            Sprite[] sprites = new Sprite[randomAtlas.spriteCount];
            randomAtlas.GetSprites(sprites);

            this.spriteLoadingSemaphore.Release();

            return sprites;
        }

        public async Awaitable<Sprite> GetRandomSpriteFromAnyCategory()
        {
            string chosenCategory = this.ChooseRandomCategory();
            IReadOnlyList<Sprite> sprites = await this.GetSpritesFromCategory(chosenCategory);
            return sprites[Random.Range(0, sprites.Count)];
        }

        public string ChooseRandomCategory()
        {
            return this.emojiCategoryLabels[Random.Range(0, this.emojiCategoryLabels.Count)];
        }

        /// <summary>
        /// Asynchronously loads a random icon, and then updates the provided profile
        /// </summary>
        /// <param name="forProfile"></param>
        /// <returns></returns>
        public static async Task AssignRandomSpriteFromAddressablesAsync(PlayerProfile forProfile)
        {
            Sprite sprite;
            string category;
            bool unique;

            do
            {
                category = Singleton.ChooseRandomCategory();
                IReadOnlyList<Sprite> sprites = await Singleton.GetSpritesFromCategory(category);
                sprite = sprites[Random.Range(0, sprites.Count)];
                unique = true;

                // Ensure no one else has this same icon
                for (int ii = 0, count = PersistentGameConfiguration.Singleton.Players.Count; ii < count; ii++)
                {
                    if (PersistentGameConfiguration.Singleton.Players[ii].SpriteStatus == PlayerProfile.PlayerProfileSpriteStatus.Saturated
                        && ReferenceEquals(PersistentGameConfiguration.Singleton.Players[ii].Sprite, sprite))
                    {
                        unique = false;
                        break;
                    }
                }
            }
            while (!unique);

            Debug.Log($"Loading saturated icon {sprite.name}");

            forProfile.UpdateSprite(sprite, category);
        }
    }
}
