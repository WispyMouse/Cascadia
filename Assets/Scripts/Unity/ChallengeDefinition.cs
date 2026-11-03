namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using Cascadia.AIs;
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Challenge.asset", menuName = "COSTCO/Challenge Definition")]
    public class ChallengeDefinition : ScriptableObject
    {
        public string ChallengeName;
        public List<AICore> AICores = new();
        public List<Sprite> AISprites = new();

        public int Width;
        public int Height;
        public int StallTurn;
        public int ForceOrderlyTurnRaw;
        public int? ForceOrderlyTurn => this.ForceOrderlyTurnRaw < 0 ? null : this.ForceOrderlyTurnRaw;

        public GameConfiguration GetGameConfiguration()
        {
            return new GameConfiguration(this.Width, this.Height, this.AICores.Count + 1, 3, this.StallTurn, this.ForceOrderlyTurn);
        }
    }
}
