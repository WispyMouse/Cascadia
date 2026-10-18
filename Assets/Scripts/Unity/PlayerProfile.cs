namespace Cascadia.Unity
{
    using UnityEngine;
    using Cascadia.AIs;
    using Cascadia.Unity.UI;

    public class PlayerProfile
    {
        public int Index;
        public string PlayerName;
        public bool IsAI;

        public PlayerProfileSprite Sprite;

        public Color KnockoutColor;

        public int AICoreIndex;

        public AICore AICore => PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex];
    }
}
