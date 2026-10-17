namespace Cascadia.DataStructures
{
    using System.Collections.Generic;
    using static Cascadia.DataStructures.BoardState;
    public interface IReadOnlyBoardState
    {
        public PlayerSide GetPlayerSideForTurnIndex(int turnIndex);
        public PlayerSide GetPlayerSideForProfileIndex(int profileIndex);
        public bool SpotIsInBounds(Coordinate position);
        public IReadOnlyList<Coordinate> GetEmptySpots();
        public MoveCommandConsequences GenerateConsequencesFromMoveCommand(MoveCommand command);
        public IReadOnlyBoardState DeepClone();
        public bool TryGetConnectionsForCoordinate(Coordinate toGet, out IReadOnlyCollection<CellsConnection> connections);
        public IReadOnlyList<CellsConnection> GetAllNewSolutions(PlayerSide sideIndex, Coordinate hypotheticalPosition);
        public bool AnyLegalMovesRemainForPlayer(PlayerSide player, out IReadOnlyList<Coordinate> possiblePlays);
        public bool StallTurnEmbargoLifted();

        public GameConfiguration BasedOnConfiguration { get; }
        public int CurrentCascadeLevel { get; }
        public int CurrentPlayerIndex { get; }
        public PlayerSide CurrentPlayerSide { get; }

        public bool IsTurnOrderStillInGame(int turnOrder);
        public IEnumerable<PlayerSide> GetSidesStillInGame();
        public PlayerSide GetCoordinateOwnership(Coordinate coordinate);
        public GameStateEnum CurrentGameState { get; }

        public BoardConnectionShapesSolver PossibilityContainer { get; }
    }
}
