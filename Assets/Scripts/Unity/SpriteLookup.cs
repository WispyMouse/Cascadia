namespace Cascadia.Unity
{
    using System.Collections.Generic;
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
    }
}
