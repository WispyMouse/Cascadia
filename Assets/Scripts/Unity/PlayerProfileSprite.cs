namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public struct PlayerProfileSprite
    {
        public Sprite Sprite;
        public string Category;
        public string SpriteName;

        public PlayerProfileSprite(Sprite sprite, string category)
        {
            this.Sprite = sprite;
            this.Category = category;

            this.SpriteName = sprite.name;
        }
    }
}
