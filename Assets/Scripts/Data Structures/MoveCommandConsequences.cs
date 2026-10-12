namespace Cascadia.DataStructures
{
    using System.Collections.Generic;

    public struct MoveCommandConsequences
    {
        public MoveCommand FromCommand;

        public IReadOnlyList<CellsConnection> ConnectionsMade;

        public IReadOnlyList<int> PlayersSidesRemoved;
        public int PreviousCascadeLevel;

        public MoveCommandConsequences(MoveCommand fromCommand, IReadOnlyList<CellsConnection> connectionsMade, int previousCascadeLevel, IReadOnlyList<int> playersSidesRemoved)
        {
            this.FromCommand = fromCommand;
            this.ConnectionsMade = connectionsMade;
            this.PreviousCascadeLevel = previousCascadeLevel;
            this.PlayersSidesRemoved = playersSidesRemoved;
        }
    }
}
