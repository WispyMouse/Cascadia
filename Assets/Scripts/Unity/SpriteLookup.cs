namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
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
            Debug.Log($"Random category chosen: {category}");

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
                throw new System.Exception($"Atlase {randomAtlas} has no sprites.");
            }

            Sprite[] sprites = new Sprite[randomAtlas.spriteCount];
            randomAtlas.GetSprites(sprites);
            return sprites;
        }

        public async Awaitable<PlayerProfileSprite> GetRandomSpriteFromAnyCategory()
        {
            string chosenCategory = this.ChooseRandomCategory();
            IReadOnlyList<Sprite> sprites = await this.GetSpritesFromCategory(chosenCategory);
            return new(sprites[Random.Range(0, sprites.Count)], chosenCategory);
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
        public static async void AssignRandomSpriteFromAddressablesAsync(PlayerProfile forProfile)
        {
            PlayerProfileSprite sprite;
            bool unique;

            do
            {
                unique = true;

                sprite = await SpriteLookup.Singleton.GetRandomSpriteFromAnyCategory();

                // Ensure no one else has this same icon
                for (int ii = 0, count = PersistentGameConfiguration.Singleton.Players.Count; ii < count; ii++)
                {
                    if (ReferenceEquals(PersistentGameConfiguration.Singleton.Players[ii].Sprite.Sprite, sprite.Sprite))
                    {
                        unique = false;
                        break;
                    }
                }
            }
            while (!unique);

            Debug.Log($"Loading saturated icon {sprite.Sprite.name}");

            forProfile.UpdateSprite(sprite);
        }
    }
}
