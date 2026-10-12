namespace Cascadia.DataStructures
{
    using System.Collections.Generic;

    /// <summary>
    /// Concept representing what a game could be started as.
    /// How large, how many players, who goes first, etc.
    /// </summary>
    public struct GameConfiguration
    {
        public int Width;
        public int Height;
        public int PlayerCount;
        public int InARowToSolve;
        public int StallTurn;

        public readonly int MaximumDimensions => this.Width * this.Height;

        /// <summary>
        /// If null, then there is no preset first-going player.
        /// The players will be randomly ordered when the game started, and a random one will go first.
        /// If this has a value, then the players will always be in index order, and the specified
        /// index will go first.
        /// </summary>
        public int? ForceOrderlyTurn;

        public GameConfiguration(int width, int height, int playerCount, int inARowToSolve, int stallTurn, int? forceOrderlyTurn)
        {
            this.Width = width;
            this.Height = height;
            this.PlayerCount = playerCount;
            this.InARowToSolve = inARowToSolve;
            this.StallTurn = stallTurn;
            this.ForceOrderlyTurn = forceOrderlyTurn;
        }

        public readonly bool SpotIsInBounds(Coordinate position)
        {
            if (position.X >= this.Width)
            {
                return false;
            }

            if (position.X < 0)
            {
                return false;
            }

            if (position.Y >= this.Height)
            {
                return false;
            }

            if (position.Y < 0)
            {
                return false;
            }

            return true;
        }

        public readonly IEnumerable<Coordinate> GetAllCoordinatesInMapDimensions()
        {
            List<Coordinate> coordinates = new(this.MaximumDimensions);

            for (int xx = 0; xx < this.Width; xx++)
            {
                for (int yy = 0; yy < this.Height; yy++)
                {
                    coordinates.Add(new Coordinate(xx, yy));
                }
            }

            return coordinates;
        }
    }
}
