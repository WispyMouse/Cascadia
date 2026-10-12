namespace Cascadia.DataStructures
{
    using System.Collections.Generic;

    public struct MoveCommand
    {
        public int Side;
        public Coordinate Position;

        public IReadOnlyList<CellsConnection> ConnectionsMade;

        public int PreviousCascadeLevel;

        public IReadOnlyList<int> PlayersSidesRemoved;

        public MoveCommand(int side, Coordinate position, IReadOnlyList<CellsConnection> connectionsMade, int previousCascade, IReadOnlyList<int> playersSidesRemoved)
        {
            this.Side = side;
            this.Position = position;
            this.ConnectionsMade = connectionsMade;
            this.PreviousCascadeLevel = previousCascade;
            this.PlayersSidesRemoved = playersSidesRemoved;
        }
    }
}
