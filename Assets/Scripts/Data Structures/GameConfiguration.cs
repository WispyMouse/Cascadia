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
                this.SaturateSquareCoordinates();
            }
        }
        private int width { get; set; }

        public int Height
        {
            readonly get => this.height;
            set
            {
                this.height = value;
                this.SaturateSquareCoordinates();
            }
        }
        private int height { get; set; }

        public int PlayerCount;
        public int InARowToSolve;
        public int StallTurn;
        public bool IsSquare { get; private set; }

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
            this.IsSquare = true;
            this.SaturateSquareCoordinates();
        }

        public GameConfiguration(IReadOnlyCollection<Coordinate> coordinates, int playerCount, int inARowToSolve, int stallTurn, int? forceOrderlyTurn)
        {
            int calculatedWidth = 0, calculatedHeight = 0;
            foreach (Coordinate coordinate in coordinates)
            {
                // A map that is 1x1 would have only one coordinate, {0, 0}
                // That map would have a width and height of 1
                // When calculating the width and height, add one to the coordinate's position
                calculatedWidth = Math.Max(calculatedWidth, coordinate.X + 1);
                calculatedHeight = Math.Max(calculatedHeight, coordinate.Y + 1);
            }
            this.width = calculatedWidth;
            this.height = calculatedHeight;

            this.IsSquare = false;
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
            return this.AllCoordinates;
        }

        /// <summary>
        /// Assuming this map is perfectly square, with no gaps in it, then we can calculate
        /// all of the coordinates that would be on the map by iterating over width and height.
        /// </summary>
        private void SaturateSquareCoordinates()
        {
            if (!this.IsSquare)
            {
                return;
            }

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

        public static GameConfiguration GetDefaultConfiguration()
        {
            return new(5, 5, 2, 3, 0, null);
        }
    }
}
