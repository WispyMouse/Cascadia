namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public struct PlayerProfileSprite
    {
        public Sprite Sprite;
        public string Category;

        public PlayerProfileSprite(Sprite sprite, string category)
        {
            this.Sprite = sprite;
            this.Category = category;
        }
    }
}
