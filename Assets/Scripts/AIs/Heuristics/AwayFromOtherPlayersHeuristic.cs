namespace Cascadia.AIs
{
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Away From Other Players Heuristic.asset", menuName = "COSTCO/AI Heuristic/Away From Player")]
    public class AwayFromOtherPlayersHeuristic : AIHeuristic
    {
        /// <summary>
        /// If there are more than this number of occupied neighbors, rate at 0.
        /// If there are this amount or fewer, rate at weight.
        /// </summary>
        public int MaximumNeighbors = 1;

        /// <summary>
        /// How distant to check
        /// </summary>
        public int Radius = 1;

        public override float ScorePosition(PlayerSide forSide, IReadOnlyBoardState currentGameState, Coordinate position)
        {
            int occupiedNeighbors = 0;

            foreach (Coordinate resultingPosition in currentGameState.PossibilityContainer.GetCoordinatesAround(currentGameState, position, 1))
            {
                if (!currentGameState.SpotIsInBounds(resultingPosition))
                {
                    continue;
                }

                PlayerSide ownership = currentGameState.SpotToSideOwnership[resultingPosition];
                if (ownership != null && ownership != forSide)
                {
                    occupiedNeighbors++;
                }
            }

            // Not too congested, ship it
            if (occupiedNeighbors <= this.MaximumNeighbors)
            {
                return this.Weight;
            }

            return 0;
        }
    }
}
