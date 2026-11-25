namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using Cascadia.AIs;
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Challenge.asset", menuName = "Cascadia/Custom Map Challenge Definition")]
    public class CustomMapChallengeDefinition : ChallengeDefinition
    {
        public List<Coordinate> Coordinates;

        public override GameConfiguration GetGameConfiguration()
        {
            return new GameConfiguration(this.Coordinates, this.AICores.Count + 1, 3, this.StallTurn, this.ForceOrderlyTurn);
        }
    }
}
