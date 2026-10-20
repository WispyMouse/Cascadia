namespace Cascadia.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Concept representing what a game could be started as.
    /// How large, how many players, who goes first, etc.
    /// </summary>
    public struct GameConfiguration
    {
        public int Width
        {
            readonly get => this.width;
            set
            {
                this.width = value;
                this.SaturateCoordinates();
            }
        }
        private int width { get; set; }

        public int Height
        {
            readonly get => this.height;
            set
            {
                this.height = value;
                this.SaturateCoordinates();
            }
        }
        private int height { get; set; }

        public int PlayerCount;
        public int InARowToSolve;
        public int StallTurn;

        public IReadOnlyCollection<Coordinate> AllCoordinates;

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
            this.width = width;
            this.height = height;

            this.PlayerCount = playerCount;
            this.InARowToSolve = inARowToSolve;
            this.StallTurn = stallTurn;
            this.ForceOrderlyTurn = forceOrderlyTurn;

            this.AllCoordinates = Array.Empty<Coordinate>();
            this.SaturateCoordinates();
        }

        public GameConfiguration(IReadOnlyCollection<Coordinate> coordinates, int playerCount, int inARowToSolve, int stallTurn, int? forceOrderlyTurn)
        {
            int width = 0, height = 0;
            foreach (Coordinate coordinate in coordinates)
            {
                width = Math.Max(width, coordinate.X);
                height = Math.Max(height, coordinate.Y);
            }
            this.width = width;
            this.height = height;

            this.AllCoordinates = coordinates;

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

            return this.AllCoordinates.Contains(position);
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

        private void SaturateCoordinates()
        {
            HashSet<Coordinate> coordinates = new(this.Width * this.Height);

            for (int xx = 0; xx < this.Width; xx++)
            {
                for (int yy = 0; yy < this.Height; yy++)
                {
                    coordinates.Add(new Coordinate(xx, yy));
                }
            }

            this.AllCoordinates = coordinates;
        }
    }
}
