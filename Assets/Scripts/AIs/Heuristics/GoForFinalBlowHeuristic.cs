namespace Cascadia.AIs
{
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Go For Final Blow Heuristic.asset", menuName = "COSTCO/AI Heuristic/Go For Final Blow Heuristic")]
    public class GoForFinalBlowHeuristic : AIHeuristic
    {
        private int mostOpponentConnections { get; set; } = 0;

        public override void BakeInformation(PlayerSide forSide, IReadOnlyBoardState currentGameState)
        {
            if (!currentGameState.StallTurnEmbargoLifted())
            {
                return;
            }

            // Determine the most number of possible connections any opponent has, anywhere
            this.mostOpponentConnections = 0;

            foreach (PlayerSide curSide in currentGameState.GetSidesStillInGame())
            {
                if (curSide == forSide)
                {
                    continue;
                }

                foreach (Coordinate playableSpot in currentGameState.GetEmptySpots())
                {
                    this.mostOpponentConnections = Mathf.Max(
                        this.mostOpponentConnections,
                        currentGameState.GenerateConsequencesFromMoveCommand(new MoveCommand(curSide, playableSpot)).ConnectionsMade.Count);
                }
            }
        }

        public override float ScorePosition(PlayerSide forSide, IReadOnlyBoardState currentGameState, Coordinate position)
        {
            if (!currentGameState.StallTurnEmbargoLifted())
            {
                return 0;
            }

            // If this spot would create more connections than any opponent can currently make, consider this
            int creatableConnections = currentGameState.GenerateConsequencesFromMoveCommand(new MoveCommand(forSide, position)).ConnectionsMade.Count;
            if (creatableConnections > this.mostOpponentConnections)
            {
                Debug.Log($"({this.name}) ({forSide}) Going for critical win against {this.mostOpponentConnections} connections max");
                return this.Weight;
            }

            return 0;
        }
    }
}
