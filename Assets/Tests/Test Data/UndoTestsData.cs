namespace Cascadia.TestData
{
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;
    using static Cascadia.TestData.TestUtilities;

    public class UndoTestsData
    {
        public class UndoTests_UndosAtEnd_DataSource_Object
        {
            public GameConfiguration GameConfiguration;
            public List<CoordinateBySide> Placements;

            public int UndosAtEnd;

            public int ExpectedPlayerIndex;
            public BoardState.GameStateEnum ExpectedGameStateEnum;
            public int ExpectedCascade;
            public int ExpectedRemainingPlayers;
            public string Description;

            public UndoTests_UndosAtEnd_DataSource_Object(string description, List<CoordinateBySide> placements, GameConfiguration configuration, int undosAtEnd,
                int expectedPlayerIndex, BoardState.GameStateEnum expectedGameStateEnum, int expectedCascade, int expectedRemainingPlayers)
            {
                this.Description = description;
                this.Placements = placements;
                this.GameConfiguration = configuration;
                this.UndosAtEnd = undosAtEnd;
                this.ExpectedPlayerIndex = expectedPlayerIndex;
                this.ExpectedGameStateEnum = expectedGameStateEnum;
                this.ExpectedCascade = expectedCascade;
                this.ExpectedRemainingPlayers = expectedRemainingPlayers;
            }

            public override string ToString() => $"({this.Description}) ({string.Join(", ", this.Placements.Select(x => x.ToString()))}) ({this.GameConfiguration.Width}x{this.GameConfiguration.Height} {this.GameConfiguration.PlayerCount} players) ({this.ExpectedPlayerIndex} player expected in {this.ExpectedGameStateEnum} with cascade {this.ExpectedCascade})";
        }

        public static UndoTests_UndosAtEnd_DataSource_Object[] UndoTests_UndosAtEnd_DataSource =
        {
            new("One action taken, undone",
                new List<CoordinateBySide>() { new (Coordinate.Zero, 0) },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 0, BoardState.GameStateEnum.NotStarted, 0, 2),

            new("Two actions taken, both undone",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Right, 1) },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 2, 0, BoardState.GameStateEnum.NotStarted, 0, 2),

            new("Two actions taken, one undone",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Right, 1) },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 1, BoardState.GameStateEnum.Playing, 0, 2),

            new("Three-in-a-row for one player in two player game, undone",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Up, 1),
                new(Coordinate.Right, 0), new(Coordinate.Up * 2, 1), new(Coordinate.Right * 2, 0) },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 0, BoardState.GameStateEnum.Playing, 0, 2),

            new("Three-in-a-row for one player, responded to with losing move, undone",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Up, 1),
                new(Coordinate.Right, 0), new(Coordinate.Up * 2, 1), new(Coordinate.Right * 2, 0), new(Coordinate.Up + Coordinate.Right, 1) },
                new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, 1, BoardState.GameStateEnum.Cascade, 1, 2),

            new($"In three-player-game, undo targets expected player after 1 undo",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Right, 1),
                new(Coordinate.Right * 2, 2) },
                new GameConfiguration(5, 5, 3, 3, 0, forceOrderlyTurn: 0), 1, 2, BoardState.GameStateEnum.Playing, 0, 3),

            new($"In three-player game, undo targets expected player after 2 undos",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Right, 1), new(Coordinate.Right * 2, 2) },
                new GameConfiguration(5, 5, 3, 3, 0, forceOrderlyTurn: 0), 2, 1, BoardState.GameStateEnum.Playing, 0, 3),

            new($"In three-player game, undo targets expected player after 6 undos",
                new List<CoordinateBySide>() { new(Coordinate.Zero, 0), new(Coordinate.Right, 1), new(Coordinate.Right * 2, 2),
                new(Coordinate.Up, 0), new(Coordinate.Up + Coordinate.Right, 1), new(Coordinate.Up + (Coordinate.Right * 2), 2) },
                new GameConfiguration(5, 5, 3, 3, 0, forceOrderlyTurn: 0), 6, 0, BoardState.GameStateEnum.NotStarted, 0, 3),
        };
    }
}
