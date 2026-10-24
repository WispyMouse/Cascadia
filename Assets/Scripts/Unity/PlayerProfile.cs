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
        public bool IsAI => this.AICoreIndex.HasValue;

        public PlayerProfileSprite Sprite;

        public Color KnockoutColor;

        public int? AICoreIndex;

        public AICore AICore => this.IsAI ? PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex.Value] : null;

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
