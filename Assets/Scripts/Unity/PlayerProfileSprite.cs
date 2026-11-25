namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public struct PlayerProfileSprite
    {
        public Sprite Sprite;
        public string Category;
        public string SpriteName;

        public PlayerProfileSprite(Sprite sprite)
        {
            this.Sprite = sprite;
            this.SpriteName = sprite.name;

            // HACK: Identify the sprite category in a more reasonable way
            this.Category = "???";
        }

        public PlayerProfileSprite(Sprite sprite, string category)
        {
            this.Sprite = sprite;
            this.Category = category;

            this.SpriteName = sprite.name;
        }
    }
}
