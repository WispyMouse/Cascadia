namespace Cascadia.AIs
{
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Random Heuristic.asset", menuName = "COSTCO/AI Heuristic/Random")]
    public class RandomHeuristic : AIHeuristic
    {
        public override float ScorePosition(PlayerSide forSide, IReadOnlyBoardState currentGameState, Coordinate position) => Random.Range(0, this.Weight);
    }
}
