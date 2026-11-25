namespace Cascadia.Unity
{
    using System;
    using Cascadia.AIs;
    using Cascadia.Unity.UI;
    using UnityEngine;

    public class PlayerProfile
    {
        public delegate void PlayerProfileDelegate(PlayerProfile profile);
        public PlayerProfileDelegate OnPlayerProfileSpriteLoaded;

        public int Index;
        public string PlayerName { get; private set; }
        public bool IsAI => this.AICoreIndex.HasValue;

        public PlayerProfileSprite? Sprite { get; private set; }
        public bool HasSprite => this.Sprite.HasValue;

        public Color KnockoutColor;

        public int? AICoreIndex { get; private set; }

        public AICore AICore => this.IsAI ? PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex.Value] : null;

        public void SetAICore(int? coreIndex, Sprite setSprite = null)
        {
            this.AICoreIndex = coreIndex;

            if (setSprite != null)
            {
                this.Sprite = new() { Sprite = setSprite, Category = "???", SpriteName = setSprite.name };
            }

            if (!coreIndex.HasValue)
            {
                this.PlayerName = PersistentGameConfiguration.HUMAN;
            }
            else if (setSprite != null)
            {
                this.PlayerName = setSprite.name;
            }
        }

        public void SetAICore(int coreIndex, Sprite sprite)
        {
            this.AICoreIndex = coreIndex;
            this.Sprite = new PlayerProfileSprite(sprite);
            this.PlayerName = this.Sprite.Value.SpriteName;
        }

        public void UpdateSprite(PlayerProfileSprite sprite)
        {
            this.Sprite = sprite;

            if (this.IsAI)
            {
                this.PlayerName = sprite.SpriteName;
            }

            if (this.OnPlayerProfileSpriteLoaded != null)
            {
                this.OnPlayerProfileSpriteLoaded.Invoke(this);

                // Clear out all listeners, to avoid having stale images trying to update when this profile updates later
                foreach (Delegate d in this.OnPlayerProfileSpriteLoaded.GetInvocationList())
                {
                    this.OnPlayerProfileSpriteLoaded -= (PlayerProfileDelegate)d;
                }
            }
        }

        public bool MatchesSprite(Sprite sprite)
        {
            if (ReferenceEquals(sprite, null))
            {
                Debug.Log($"Asked to compare this sprite to a null sprite. How odd!");
                return false;
            }

            return ReferenceEquals(this.Sprite.Value.Sprite, sprite) || string.Equals(this.Sprite.Value.SpriteName, sprite.name);
        }

        public void SetName(string newName)
        {
            this.PlayerName = newName;
        }
    }
}
