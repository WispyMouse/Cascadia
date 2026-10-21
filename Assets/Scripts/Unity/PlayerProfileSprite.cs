namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public struct PlayerProfileSprite
    {
        public bool NeedsResolution;
        public Sprite Sprite;
        public string Category;
        public string SpriteName;

        public PlayerProfileSprite(Sprite sprite, string category)
        {
            this.NeedsResolution = sprite == null;
            this.Sprite = sprite;
            this.Category = category;

            if (this.NeedsResolution)
            {
                this.SpriteName = null;
            }
            else
            {
                this.SpriteName = sprite.name;
            }
        }

        public static PlayerProfileSprite GetNeedsResolution()
        {
            return new PlayerProfileSprite(null, null);
        }
    }
}
