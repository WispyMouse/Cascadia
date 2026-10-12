namespace Cascadia.DataStructures
{
    public struct MoveCommand
    {
        public int Side;
        public Coordinate Position;

        public MoveCommand(int side, Coordinate position)
        {
            this.Side = side;
            this.Position = position;
        }
    }
}
