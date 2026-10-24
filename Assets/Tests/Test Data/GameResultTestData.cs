namespace Cascadia.TestData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cascadia.DataStructures;
    using static Cascadia.TestData.TestUtilities;

    public static class GameResultTestData
    {
        public readonly struct CoordinatesToPlay_DataSource_Object
        {
            public readonly GameConfiguration GameConfiguration;
            public readonly IReadOnlyList<CoordinateBySide> Coordinates;
            public readonly int? Winner;

            public CoordinatesToPlay_DataSource_Object(GameConfiguration configuration, IReadOnlyList<CoordinateBySide> coordinates, int? winner)
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
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1),
                    new(new(1, 0), 0), new(new(0, 2), 1),
                    new(new(2, 0), 0), new(new(1, 1), 1),
            }, 0),

            // O moves three in a row, X responds, X should win because there are no more moves
            new(new GameConfiguration(3, 2, 2, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1),
                    new(new(1, 0), 0), new(new(1, 1), 1),
                    new(new(2, 0), 0), new(new(2, 1), 1),
            }, 1),

            // O moves three in a row, X responds, Y responds, Y should win because there are no more moves
            new(new GameConfiguration(3, 3, 3, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1), new(new(0, 2), 2),
                    new(new(1, 0), 0), new(new(1, 1), 1), new(new(1, 2), 2),
                    new(new(2, 0), 0), new(new(2, 1), 1), new(new(2, 2), 2),
            }, 2),

            // O moves three in a row, X flops, Y flops, O should win
            new(new GameConfiguration(3, 3, 3, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1), new(new(0, 2), 2),
                    new(new(1, 0), 0), new(new(1, 2), 1), new(new(1, 1), 2),
                    new(new(2, 0), 0), new(new(2, 1), 1), new(new(2, 2), 2),
            }, 0),

            // No one can make a connection on this tiny board, so no one wins
            new(new GameConfiguration(2, 2, 2, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1),
                    new(new(1, 0), 0), new(new(1, 1), 1),
            }, null),

            // No one can make a connection on this tiny board, so no one wins
            // There are as many players as there are spots
            new(new GameConfiguration(2, 3, 6, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1), new(new(0, 2), 2),
                    new(new(1, 0), 3), new(new(1, 1), 4), new(new(1, 2), 5),
            }, null),

            // There are twice as many spots, but that's still not enough for anyone to win
            new(new GameConfiguration(2, 6, 6, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(0, 0), 0), new(new(0, 1), 1), new(new(0, 2), 2),
                    new(new(1, 0), 3), new(new(1, 1), 4), new(new(1, 2), 5),

                    new(new(0, 3), 0), new(new(0, 4), 1), new(new(0, 5), 2),
                    new(new(1, 3), 3), new(new(1, 4), 4), new(new(1, 5), 5),
            }, null),

            // Matches made in a larger board for two players
            new(new GameConfiguration(10, 10, 2, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(9, 0), 0), new(new(9, 1), 1),
                    new(new(8, 0), 0), new(new(8, 1), 1),
                    new(new(7, 0), 0), new(new(7, 1), 1),
                    new(new(6, 0), 0)
            }, 1),

            // Matches made in a larger board for four players
            new(new GameConfiguration(10, 10, 4, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(9, 0), 0), new(new(9, 1), 1), new(new(9, 2), 2), new(new(9, 3), 3),
                    new(new(8, 0), 0), new(new(8, 1), 1), new(new(8, 2), 2), new(new(8, 3), 3),
                    new(new(7, 0), 0), new(new(7, 1), 1), new(new(7, 2), 2), new(new(7, 3), 3),
                    new(new(6, 0), 0), new(new(6, 1), 1), new(new(6, 2), 2)
            }, 3),

            new(new GameConfiguration(10, 10, 4, 3, 0, forceOrderlyTurn: 0),
                new List<CoordinateBySide>()
            {
                    new(new(9, 0), 0), new(new(9, 1), 1), new(new(9, 2), 2), new(new(9, 3), 3),
                    new(new(8, 0), 0), new(new(8, 1), 1), new(new(8, 2), 2), new(new(8, 3), 3),
                    new(new(7, 0), 0), new(new(7, 1), 1), new(new(7, 9), 2), new(new(7, 3), 3), // Third player misses connection
                    new(new(6, 0), 0), new(new(6, 1), 1)  // Unable to make connections, player index 2 wins 
            }, 3),
        };

        /// <summary>
        /// Assuming a range of 0-
        /// Taken from SergeyS and AustinWBryan's Stack Overflow answer
        /// https://stackoverflow.com/a/19891145
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static int[][] FastPowerSet(int range)
        {
            int[] seq = Enumerable.Range(0, range).ToArray();

            int[][] powerSet = new int[1 << seq.Length][];
            powerSet[0] = new int[0]; // starting only with empty set

            for (int i = 0; i < seq.Length; i++)
            {
                int cur = seq[i];
                int count = 1 << i; // doubling list each time
                for (int j = 0; j < count; j++)
                {
                    int[] source = powerSet[j];
                    int[] destination = powerSet[count + j] = new int[source.Length + 1];

                    for (int q = 0; q < source.Length; q++)
                    {
                        destination[q] = source[q];
                    }
                    destination[source.Length] = cur;
                }
            }
            return powerSet;
        }
    }
}
