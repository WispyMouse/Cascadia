namespace Cascadia.TestData
{
    using System.Collections;
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using UnityEditor;
    using UnityEngine;

    public class TestUtilities
    {
        public struct CoordinateBySide
        {
            public Coordinate Placement;
            public int Side;

            public CoordinateBySide(Coordinate coordinate, int side)
            {
                this.Placement = coordinate;
                this.Side = side;
            }
        }

        /// <summary>
        /// If were you to start up a game, how many players could possibly be playing?
        /// This enumerates each possibility.
        /// </summary>
        public static IEnumerable<int> PlayerCountConfigurations
        {
            get
            {
                List<int> playerCounts = new();

                for (int ii = BoardState.MINIMUMPLAYERS; ii < BoardState.MAXIMUMPLAYERS; ii++)
                {
                    playerCounts.Add(ii);
                }

                return playerCounts;
            }
        }
    }
}
