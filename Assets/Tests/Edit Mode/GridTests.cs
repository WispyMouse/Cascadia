namespace Cascadia.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;

    public class GridTests
    {
        public class PlacementCausesSolve_DataSource_Object
        {
            public GameConfiguration GameConfiguration;
            public List<Coordinate> Placements;
            public int ExpectedSolutions;

            public PlacementCausesSolve_DataSource_Object(List<Coordinate> placements, GameConfiguration gameConfiguration, int expectedSolutions)
            {
                this.Placements = placements;
                this.GameConfiguration = gameConfiguration;
                this.ExpectedSolutions = expectedSolutions;
            }

            public override string ToString() => $"({string.Join(", ", this.Placements.Select(x => x.ToString()))}) ({this.GameConfiguration.Width}x{this.GameConfiguration.Height}) ({this.ExpectedSolutions} expected)";
        }

        public static PlacementCausesSolve_DataSource_Object[] PlacementCausesSolve_DataSource =
        {
        // Simple single test cases
        new (
            new List<Coordinate>() { Coordinate.Right, Coordinate.Right + Coordinate.Right, Coordinate.Zero, },
            new GameConfiguration(3, 3, 1, 3, 0, forceOrderlyTurn: 0), 1),
        new (
            new List<Coordinate>() { Coordinate.Right, Coordinate.Zero, Coordinate.Right + Coordinate.Right },
            new GameConfiguration(3, 3, 1, 3, 0, forceOrderlyTurn: 0), 1),
        new (
            new List<Coordinate>() { Coordinate.Zero, Coordinate.Up, Coordinate.Up * 2 },
            new GameConfiguration(3, 3, 1, 3, 0, forceOrderlyTurn: 0), 1),
        new (
            new List<Coordinate>() { Coordinate.Right, Coordinate.Right + Coordinate.Right + Coordinate.Up, (Coordinate.Up * 2) + (Coordinate.Right * 3) },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 1),

        // This shouldn't be a score
        new (
            new List<Coordinate>() { Coordinate.Right + Coordinate.Up, Coordinate.Right + Coordinate.Up + Coordinate.Up, Coordinate.Zero },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 0),

        // Double cascade
        new (
            new List<Coordinate>() { Coordinate.Right * 2, Coordinate.Right, Coordinate.Up, Coordinate.Up * 2, Coordinate.Zero },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 2),

        // Quadruple Cascade
        new (
            new List<Coordinate>() { 
                // Diagonal up/right
                Coordinate.Zero, Coordinate.Right + Coordinate.Up, (Coordinate.Right + Coordinate.Up) * 3, (Coordinate.Right + Coordinate.Up) * 4,
                // Diagonal up/left
                Coordinate.Right * 4, (Coordinate.Right * 3) + Coordinate.Up, Coordinate.Right + (Coordinate.Up * 3), Coordinate.Up * 4,
                // Horizontal
                Coordinate.Up * 2, (Coordinate.Up * 2) + Coordinate.Right, (Coordinate.Up * 2) + (Coordinate.Right * 3), (Coordinate.Up * 2) + (Coordinate.Right * 4),
                // Vertical
                Coordinate.Right * 2, (Coordinate.Right * 2) + Coordinate.Up, (Coordinate.Right * 2) + (Coordinate.Up * 3), (Coordinate.Right * 2) + (Coordinate.Up * 4),
                // Pop it!
                (Coordinate.Up * 2) + (Coordinate.Right * 2)
            },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 4),

        // 4-in-a-row should count as one solution
        new (
            new List<Coordinate>() { Coordinate.Zero, Coordinate.Right * 2, Coordinate.Right * 3, Coordinate.Right },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 1),

        // 5-in-a-row should count as one solution
        new (
            new List<Coordinate>() { Coordinate.Zero, Coordinate.Right, Coordinate.Right * 3, Coordinate.Right * 4, Coordinate.Right * 2 },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 1),

        // Shouldn't count as a new solution if you continue an old 3-of
        new (
            new List<Coordinate>() { Coordinate.Right, Coordinate.Right * 2, Coordinate.Right * 3, Coordinate.Right * 4 },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 0),

        // Shouldn't care that there was a previously made connection
        new (
            new List<Coordinate>() { Coordinate.Right, Coordinate.Right * 2, Coordinate.Right * 3, Coordinate.Up + Coordinate.Right, Coordinate.Up + (Coordinate.Right * 2) },
            new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 0),
    };

        [Test]
        [TestCaseSource(nameof(PlacementCausesSolve_DataSource))]
        public void PlacingThirdWouldResultInRow(PlacementCausesSolve_DataSource_Object plan)
        {
            int lastIndex = plan.Placements.Count - 1;
            BoardState testState = new(plan.GameConfiguration);

            for (int ii = 0; ii < lastIndex; ii++)
            {
                Assert.IsTrue(testState.TryApplyMoveCommand(new MoveCommand(0, plan.Placements[ii]), false));
            }

            MoveCommandConsequences lastCommand = testState.GenerateConsequencesFromMoveCommand(new MoveCommand(0, plan.Placements[lastIndex]));
            Assert.AreEqual(plan.ExpectedSolutions, lastCommand.ConnectionsMade.Count, $"Especting a specific amount of solutions total");

            Assert.IsTrue(testState.TryApplyMoveCommand(lastCommand.FromCommand, false));

            if (plan.ExpectedSolutions > 0)
            {
                foreach (Coordinate position in plan.Placements)
                {
                    if (!testState.TryGetConnectionsForCoordinate(position, out _))
                    {
                        Assert.Fail($"There should be an accepted solution for {position}.");
                    }
                }
            }
        }
    }

}
