namespace Cascadia.Unity
{
    using System;
    using System.Collections.Generic;
    using Cascadia.AIs;
    using Cascadia.Unity.UI;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class PlayerProfile
    {
        public enum PlayerProfileSpriteStatus
        {
            NotRequested = 0,
            Requested = 1,
            Saturated = 2
        }

        /// <summary>
        /// When a PlayerProfile is initially setup, it may not yet have a Sprite.
        /// If there's already a Sprite, we can pop this immediately.
        /// If there isn't already a sprite, we'll pop it when the Sprite loads.
        /// </summary>
        private readonly UnityEvent<Sprite, string> pendingSpriteLoadActions = new();

        public int Index;
        public string PlayerName { get; private set; }
        public bool IsAI => this.AICoreIndex.HasValue;

        public Sprite Sprite { get; private set; }
        public string SpriteCategory { get; private set; }
        public PlayerProfileSpriteStatus SpriteStatus { get; set; } = PlayerProfileSpriteStatus.NotRequested;

        public Color KnockoutColor;

        public int? AICoreIndex { get; private set; }

        public AICore AICore => this.IsAI ? PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex.Value] : null;

        public void SetAICore(int? coreIndex, Sprite setSprite = null)
        {
            this.AICoreIndex = coreIndex;

            if (setSprite != null)
            {
                this.Sprite = setSprite;
                this.SpriteStatus = PlayerProfileSpriteStatus.Saturated;
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
            this.Sprite = sprite;
            this.PlayerName = this.Sprite.name;
            this.SpriteStatus = PlayerProfileSpriteStatus.Saturated;
            this.SpriteCategory = "???";
        }

        public void AssignSpriteWhenReady(Image imageToApply)
        {
            if (this.SpriteStatus == PlayerProfileSpriteStatus.Saturated)
            {
                imageToApply.sprite = this.Sprite;
                return;
            }

            this.SpriteStatus = PlayerProfileSpriteStatus.Requested;

            this.pendingSpriteLoadActions.AddListener((newSprite, category) =>
            {
                // Null check in case the image is not no longer with us
                if (imageToApply == null)
                {
                    return;
                }

                this.SpriteCategory = category;
                imageToApply.sprite = newSprite;
            });
        }

        public void AssignSpriteWhenReady(SpriteRenderer spriteRendererToApply)
        {
            if (this.SpriteStatus == PlayerProfileSpriteStatus.Saturated)
            {
                spriteRendererToApply.sprite = this.Sprite;
                return;
            }

            this.SpriteStatus = PlayerProfileSpriteStatus.Requested;

            this.pendingSpriteLoadActions.AddListener((newSprite, category) =>
            {
                // Null check in case the image is not no longer with us
                if (spriteRendererToApply == null)
                {
                    return;
                }

                this.SpriteCategory = category;
                spriteRendererToApply.sprite = newSprite;
            });
        }

        public void UpdateSprite(Sprite sprite, string category)
        {
            this.Sprite = sprite;
            this.SpriteCategory = category;

            if (this.IsAI)
            {
                this.PlayerName = sprite.name;
            }

            this.SpriteStatus = PlayerProfileSpriteStatus.Saturated;
            this.pendingSpriteLoadActions?.Invoke(sprite, category);
            this.pendingSpriteLoadActions?.RemoveAllListeners();
        }

        public bool MatchesSprite(Sprite sprite)
        {
            if (ReferenceEquals(sprite, null))
            {
                Debug.Log($"Asked to compare this sprite to a null sprite. How odd!");
                return false;
            }

            return ReferenceEquals(this.Sprite, sprite) || string.Equals(this.Sprite.name, sprite.name);
        }

        public void SetName(string newName)
        {
            this.PlayerName = newName;
        }
    }
}
