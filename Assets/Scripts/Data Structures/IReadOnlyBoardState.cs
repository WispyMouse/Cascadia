namespace Cascadia.DataStructures
{
    using System.Collections.Generic;
    using static Cascadia.DataStructures.BoardState;
    public interface IReadOnlyBoardState
    {
        public bool SpotIsInBounds(Coordinate position);
        public IReadOnlyList<Coordinate> GetEmptySpots();
        public MoveCommandConsequences GenerateConsequencesFromMoveCommand(MoveCommand command);
        public IReadOnlyBoardState DeepClone();
        public bool TryGetConnectionsForCoordinate(Coordinate toGet, out IReadOnlyCollection<CellsConnection> connections);
        public IReadOnlyList<CellsConnection> GetAllNewSolutions(int sideIndex, Coordinate hypotheticalPosition);
        public bool AnyLegalMovesRemainForPlayer(int player, out IReadOnlyList<Coordinate> possiblePlays);
        public bool StallTurnEmbargoLifted();

        public int Height { get; }
        public int Width { get; }
        public int PlayerCount { get; }
        public int InARowToSolve { get; }
        public int CurrentCascadeLevel { get; }
        public int CurrentPlayerIndex { get; }
        public PlayerSide CurrentPlayerSide { get; }

        public bool IsTurnOrderStillInGame(int turnOrder);
        public IEnumerable<PlayerSide> GetSidesStillInGame();
        public IReadOnlyDictionary<Coordinate, int?> SpotToSideOwnership { get; }
        public GameStateEnum CurrentGameState { get; }

        public BoardConnectionShapesSolver PossibilityContainer { get; }
    }
}
