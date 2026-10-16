namespace Cascadia.DataStructures
{
    using System.Collections.Generic;

    public struct MoveCommandConsequences
    {
        public MoveCommand FromCommand;

        public IReadOnlyList<CellsConnection> ConnectionsMade;

        public IReadOnlyCollection<PlayerSide> PlayerSidesRemoved;
        public int PreviousCascadeLevel;

        public MoveCommandConsequences(MoveCommand fromCommand, IReadOnlyList<CellsConnection> connectionsMade, int previousCascadeLevel, IReadOnlyCollection<PlayerSide> playerSidesRemoved)
        {
            this.FromCommand = fromCommand;
            this.ConnectionsMade = connectionsMade;
            this.PreviousCascadeLevel = previousCascadeLevel;
            this.PlayerSidesRemoved = playerSidesRemoved;
        }
    }
}
