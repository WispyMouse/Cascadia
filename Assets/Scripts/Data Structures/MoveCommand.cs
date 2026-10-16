namespace Cascadia.DataStructures
{
    public struct MoveCommand
    {
        public PlayerSide Side;
        public Coordinate Position;

        public MoveCommand(PlayerSide side, Coordinate position)
        {
            this.Side = side;
            this.Position = position;
        }
    }
}
