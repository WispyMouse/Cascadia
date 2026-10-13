namespace Cascadia.DataStructures
{
    using System.Collections.Generic;

    public struct MoveCommandConsequences
    {
        public MoveCommand FromCommand;

        public IReadOnlyList<CellsConnection> ConnectionsMade;

        public IEnumerable<int> TurnOrdersRemoved;
        public int PreviousCascadeLevel;

        public MoveCommandConsequences(MoveCommand fromCommand, IReadOnlyList<CellsConnection> connectionsMade, int previousCascadeLevel, IEnumerable<int> turnOrdersRemoved)
        {
            this.FromCommand = fromCommand;
            this.ConnectionsMade = connectionsMade;
            this.PreviousCascadeLevel = previousCascadeLevel;
            this.TurnOrdersRemoved = turnOrdersRemoved;
        }
    }
}
