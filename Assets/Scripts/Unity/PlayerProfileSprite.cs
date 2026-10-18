namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public struct PlayerProfileSprite
    {
        public bool NeedsResolution;
        public Sprite Sprite;
        public string Category;

        public PlayerProfileSprite(Sprite sprite, string category)
        {
            this.NeedsResolution = sprite == null;
            this.Sprite = sprite;
            this.Category = category;
        }

        public static PlayerProfileSprite GetNeedsResolution()
        {
            return new PlayerProfileSprite(null, null);
        }
    }
}
