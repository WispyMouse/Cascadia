namespace Cascadia.Unity
{
    using UnityEngine;
    using Cascadia.AIs;
    using Cascadia.Unity.UI;

    public class PlayerProfile
    {
        public delegate void PlayerProfileSpriteUpdated();
        public PlayerProfileSpriteUpdated OnPlayerProfileSpriteUpdated;

        public int Index;
        public string PlayerName;
        public bool IsAI;

        public PlayerProfileSprite Sprite;

        public Color KnockoutColor;

        public int AICoreIndex;

        public AICore AICore => PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex];

        public void UpdateSprite(PlayerProfileSprite sprite)
        {
            this.Sprite = sprite;
            this.OnPlayerProfileSpriteUpdated?.Invoke();
        }

        public bool MatchesSprite(Sprite sprite)
        {
            return ReferenceEquals(this.Sprite.Sprite, sprite) || string.Equals(this.Sprite.SpriteName, sprite.name);
        }
    }
}
