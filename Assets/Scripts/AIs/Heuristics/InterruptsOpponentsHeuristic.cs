namespace Cascadia.AIs
{
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Interupts Opponent Heuristic.asset", menuName = "COSTCO/AI Heuristic/Interrupts Opponent")]
    public class InterruptsOpponentHeuristic : AIHeuristic
    {
        /// <summary>
        /// How many connections this needs to interrupt to apply
        /// </summary>
        public int MinimumConnections = 1;

        public override float ScorePosition(PlayerSide forSide, IReadOnlyBoardState currentGameState, Coordinate position)
        {
            int connectionsOpponentsCanMake = 0;

            foreach (PlayerSide remainingSide in currentGameState.GetSidesStillInGame())
            {
                if (remainingSide == forSide)
                {
                    continue;
                }

                connectionsOpponentsCanMake += currentGameState.GetAllNewSolutions(remainingSide, position).Count;
            }

            if (connectionsOpponentsCanMake >= this.MinimumConnections)
            {
                return this.Weight;
            }
            else
            {
                return 0;
            }
        }
    }
}
