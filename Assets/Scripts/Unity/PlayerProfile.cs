namespace Cascadia.Unity
{
    using UnityEngine;
    using Cascadia.AIs;

    public class PlayerProfile
    {
        public int Index;
        public string PlayerName;
        public bool IsAI;
        public int RepresenterSpriteIndex;
        public Color KnockoutColor;

        public int AICoreIndex;

        public Sprite SpriteRepresentation => PersistentGameConfiguration.Singleton.PlayerIconOptions[this.RepresenterSpriteIndex];

        public AICore AICore => PersistentGameConfiguration.Singleton.AICores[this.AICoreIndex];
    }
}
