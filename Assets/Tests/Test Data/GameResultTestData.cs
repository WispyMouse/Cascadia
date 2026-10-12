namespace Cascadia.TestData
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;

    public static class GameResultTestData
    {
        public class CoordinatesToPlay_DataSource_Object
        {
            public GameConfiguration GameConfiguration;
            public List<Coordinate> Coordinates;
            public int? Winner;

            public CoordinatesToPlay_DataSource_Object(GameConfiguration configuration, List<Coordinate> coordinates, int? winner)
            {
                this.GameConfiguration = configuration;
                this.Coordinates = coordinates;
                this.Winner = winner;
            }

            public override string ToString() => $"{this.GameConfiguration.PlayerCount} players over {this.Coordinates.Count} moves: winner: {this.Winner}";
        }

        public static CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource =
        {
            // O moves three in a row and thus should win
            new(new GameConfiguration(3, 3, 2, 3, 0, forceOrderlyTurn: 0),
                new List<Coordinate>()
            {
                    new(0, 0), new(0, 1),
                    new(1, 0), new(0, 2),
                    new(2, 0), new(1, 1),
            }, 0),

            // O moves three in a row, X responds, X should win because there are no more moves
            new(new GameConfiguration(3, 2, 2, 3, 0, forceOrderlyTurn: 0),
                new List<Coordinate>()
            {
                    new(0, 0), new(0, 1),
                    new(1, 0), new(1, 1),
                    new(2, 0), new(2, 1),
            }, 1),

            // O moves three in a row, X responds, Y responds, Y should win because there are no more moves
            new(new GameConfiguration(3, 3, 3, 3, 0, forceOrderlyTurn: 0),
                new List<Coordinate>()
            {
                    new(0, 0), new(0, 1), new(0, 2),
                    new(1, 0), new(1, 1), new(1, 2),
                    new(2, 0), new(2, 1), new(2, 2),
            }, 2),

            // O moves three in a row, X flops, Y flops, O should win
            new(new GameConfiguration(3, 3, 3, 3, 0, forceOrderlyTurn: 0),
                new List<Coordinate>()
            {
                    new(0, 0), new(0, 1), new(0, 2),
                    new(1, 0), new(1, 2), new(1, 1),
                    new(2, 0), new(2, 1), new(2, 2),
            }, 0),

            // No one can make a connection on this tiny board, so no one wins
            new(new GameConfiguration(2, 2, 2, 3, 0, forceOrderlyTurn: 0),
                new List<Coordinate>()
            {
                    new(0, 0), new(0, 1),
                    new(1, 0), new(1, 1),
            }, null),
        };
    }
}
