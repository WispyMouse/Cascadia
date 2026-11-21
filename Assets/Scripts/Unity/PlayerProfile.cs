namespace Cascadia.Unity
{
    using UnityEngine;
    using Cascadia.AIs;
    using Cascadia.Unity.UI;

    public class PlayerProfile
    {
        public int Index;
        public string PlayerName;
        public bool IsAI => this.AICoreIndex.HasValue;

        public PlayerProfileSprite Sprite;

        public Color KnockoutColor;

        public int? AICoreIndex { get; private set; }

        public AICore AICore => this.IsAI ? PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex.Value] : null;

        public void SetAICore(int? coreIndex)
        {
            this.AICoreIndex = coreIndex;
        }

        public void UpdateSprite(PlayerProfileSprite sprite)
        {
            this.Sprite = sprite;
            if (this.AICore != null)
            {
                this.PlayerName = sprite.SpriteName;
            }
        }

        public bool MatchesSprite(Sprite sprite)
        {
            if (ReferenceEquals(sprite, null))
            {
                Debug.Log($"Asked to compare this sprite to a null sprite. How odd!");
                return false;
            }

            return ReferenceEquals(this.Sprite.Sprite, sprite) || string.Equals(this.Sprite.SpriteName, sprite.name);
        }
    }
}
