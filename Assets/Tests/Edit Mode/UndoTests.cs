namespace Cascadia.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;

    public class UndoTests
    {
        public class UndoTests_UndosAtEnd_DataSource_Object
        {
            public GameConfiguration GameConfiguration;
            public List<Coordinate> Placements;

            public int UndosAtEnd;

            public int ExpectedPlayerIndex;
            public BoardState.GameStateEnum ExpectedGameStateEnum;
            public int ExpectedCascade;
            public int ExpectedRemainingPlayers;

            public UndoTests_UndosAtEnd_DataSource_Object(List<Coordinate> placements, GameConfiguration configuration, int undosAtEnd,
                int expectedPlayerIndex, BoardState.GameStateEnum expectedGameStateEnum, int expectedCascade, int expectedRemainingPlayers)
            {
                this.Placements = placements;
                this.GameConfiguration = configuration;
                this.UndosAtEnd = undosAtEnd;
                this.ExpectedPlayerIndex = expectedPlayerIndex;
                this.ExpectedGameStateEnum = expectedGameStateEnum;
                this.ExpectedCascade = expectedCascade;
                this.ExpectedRemainingPlayers = expectedRemainingPlayers;
            }

            public override string ToString() => $"({string.Join(", ", this.Placements.Select(x => x.ToString()))}) ({this.GameConfiguration.Width}x{this.GameConfiguration.Height} {this.GameConfiguration.PlayerCount} players) ({this.ExpectedPlayerIndex} player expected in {this.ExpectedGameStateEnum} with cascade {this.ExpectedCascade})";
        }

        public static UndoTests_UndosAtEnd_DataSource_Object[] UndoTests_UndosAtEnd_DataSource =
        {
            // One action taken, undone
            new(new List<Coordinate>() { Coordinate.Zero },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 0, BoardState.GameStateEnum.NotStarted, 0, 2),

            // Two actions taken, both undone
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Right },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 2, 0, BoardState.GameStateEnum.NotStarted, 0, 2),

            // Two actions taken, one undone
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Right },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 1, BoardState.GameStateEnum.Playing, 0, 2),

            // Three-in-a-row for one player, undone
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Right, Coordinate.Right * 2 },
                new GameConfiguration(5, 5, 1, 3, 0, forceOrderlyTurn: 0), 1, 0, BoardState.GameStateEnum.Playing, 0, 1),

            // Three-in-a-row for one player in two player game, undone
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Up, Coordinate.Right, Coordinate.Up * 2, Coordinate.Right * 2 },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 0, BoardState.GameStateEnum.Playing, 0, 2),

            // Three-in-a-row for one player, responded to with losing move, undone
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Up, Coordinate.Right, Coordinate.Up * 2, Coordinate.Right * 2, Coordinate.Up + Coordinate.Right },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 1, BoardState.GameStateEnum.Cascade, 1, 2),

            // In three-player-game, undo targets expected player
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Right, Coordinate.Right * 2 },
                new GameConfiguration(5, 5, 3, 3, 0, forceOrderlyTurn: 0), 1, 2, BoardState.GameStateEnum.Playing, 0, 3),
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Right, Coordinate.Right * 2 },
                new GameConfiguration(5, 5, 3, 3, 0, forceOrderlyTurn: 0), 2, 1, BoardState.GameStateEnum.Playing, 0, 3),
            new(new List<Coordinate>() { Coordinate.Zero, Coordinate.Right, Coordinate.Right * 2, Coordinate.Up, Coordinate.Up + Coordinate.Right, Coordinate.Up + (Coordinate.Right * 2) },
                new GameConfiguration(5, 5, 3, 3, 0, forceOrderlyTurn: 0), 6, 0, BoardState.GameStateEnum.NotStarted, 0, 3),
        };

        [Test]
        [TestCaseSource(nameof(UndoTests_UndosAtEnd_DataSource))]
        public void KnockoutStatusAsExpected(UndoTests_UndosAtEnd_DataSource_Object plan)
        {
            BoardState testState = new(plan.GameConfiguration);
            List<PlayerSide> expectedOwnership = new();

            for (int ii = 0, count = plan.Placements.Count; ii < count; ii++)
            {
                Assert.IsTrue(testState.TryApplyMoveCommand(new MoveCommand(testState.CurrentPlayerSide, plan.Placements[ii]), true));
                expectedOwnership.Add(testState.SpotToSideOwnership[plan.Placements[ii]]);
            }

            for (int ii = 0; ii < plan.UndosAtEnd; ii++)
            {
                testState.ReversePreviousMoveCommand();
            }

            Assert.AreEqual(plan.ExpectedGameStateEnum, testState.CurrentGameState, $"Expecting game state to be in specific status");
            Assert.AreEqual(plan.ExpectedPlayerIndex, testState.CurrentPlayerIndex, $"Expecting current player index to be specific");
            Assert.AreEqual(plan.ExpectedCascade, testState.CurrentCascadeLevel, $"Expecting cascade to be specific");
            Assert.AreEqual(plan.ExpectedRemainingPlayers, testState.PlayerSidesStillInGame.Count, $"Expecting remaining player count to be specific");

            for (int ii = 0, count = plan.Placements.Count - plan.UndosAtEnd; ii < count; ii++)
            {
                Assert.IsTrue(testState.SpotToSideOwnership[plan.Placements[ii]] == expectedOwnership[ii], "Expecting ownership to still be claimed");
            }

            for (int ii = plan.Placements.Count - plan.UndosAtEnd + 1, count = plan.Placements.Count; ii < count; ii++)
            {
                Assert.IsTrue(testState.SpotToSideOwnership[plan.Placements[ii]] == null, "Expecting ownership to now be empty");
            }
        }
    }

}
