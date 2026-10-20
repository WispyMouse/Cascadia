namespace Cascadia.TestData
{
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;
    using static Cascadia.TestData.TestUtilities;

    public class KnockoutTestsData
    {
        public class Knockout_ExpectedPlayer_DataSource_Object
        {
            public GameConfiguration GameConfiguration;
            public List<CoordinateBySide> Placements;

            public int ExpectedPlayerIndex;
            public BoardState.GameStateEnum ExpectedGameStateEnum;
            public int ExpectedCascade;
            public IReadOnlyCollection<int> ExpectedRemainingPlayers;

            public Knockout_ExpectedPlayer_DataSource_Object(List<CoordinateBySide> placements, GameConfiguration configuration, int expectedPlayerIndex,
                BoardState.GameStateEnum expectedGameStateEnum, int expectedCascade, IReadOnlyCollection<int> expectedRemainingPlayers)
            {
                this.Placements = placements;
                this.GameConfiguration = configuration;
                this.ExpectedPlayerIndex = expectedPlayerIndex;
                this.ExpectedGameStateEnum = expectedGameStateEnum;
                this.ExpectedCascade = expectedCascade;
                this.ExpectedRemainingPlayers = expectedRemainingPlayers;
            }

            public override string ToString() => $"({string.Join(", ", this.Placements.Select(x => x.ToString()))}) ({this.GameConfiguration.Width}x{this.GameConfiguration.Height} {this.GameConfiguration.PlayerCount} players) ({this.ExpectedPlayerIndex} player expected in {this.ExpectedGameStateEnum} with cascade {this.ExpectedCascade})";
        }

        public static Knockout_ExpectedPlayer_DataSource_Object[] Knockout_ExpectedPlayer_DataSource =
        {
            // Early in the game, it's still in playing state
            new(new List<CoordinateBySide>()
            {
                new(Coordinate.Zero, 0), new(Coordinate.Up, 1), new(Coordinate.Right, 0),
            }, new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, BoardState.GameStateEnum.Playing, 0,
                expectedRemainingPlayers: new List<int>() { 0, 1 }),

            // Player 1 places three in a row while Player 2 doesn't
            // On Player 2's response turn, the cascade should be set, but the game isn't over
            new(new List<CoordinateBySide>()
            {
                new(Coordinate.Zero, 0), new(Coordinate.Up, 1), new(Coordinate.Right, 0),
                new(Coordinate.Up + Coordinate.Right, 1), new(Coordinate.Right * 2, 0)
            }, new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, BoardState.GameStateEnum.Cascade, 1,
                expectedRemainingPlayers: new List<int>() { 0, 1 }),
            
            // Player 2 makes a move that ends up losing them the game
            new(new List<CoordinateBySide>()
            {
                new(Coordinate.Zero, 0), new(Coordinate.Up, 1), new(Coordinate.Right, 0),
                new(Coordinate.Up + Coordinate.Right, 1), new(Coordinate.Right * 2, 0), new(Coordinate.Up * 2, 1)
            }, new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 0, BoardState.GameStateEnum.End, 1,
                expectedRemainingPlayers: new List<int>() { 0 }),

            // Player 2 makes a move that keeps them in the game by placing three in a row
            new(new List<CoordinateBySide>()
            {
                new(Coordinate.Zero, 0), new(Coordinate.Up, 1), new(Coordinate.Right, 0),
                new(Coordinate.Up + Coordinate.Right, 1), new(Coordinate.Right * 2, 0),
                new(Coordinate.Up + (Coordinate.Right * 2), 1)
            }, new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 0, BoardState.GameStateEnum.Cascade, 1,
                expectedRemainingPlayers: new List<int>() { 0, 1 }),

            // Player 0 responds with a move that should lose them the game
            new(new List<CoordinateBySide>()
            {
                new(Coordinate.Zero, 0), new(Coordinate.Up, 1), new(Coordinate.Right, 0),
                new(Coordinate.Up + Coordinate.Right, 1), new(Coordinate.Right * 2, 0),
                new(Coordinate.Up + (Coordinate.Right * 2), 1),

                // Should not make a new cascade, thus should make the first player lose
                new(Coordinate.Right * 4, 0)
            }, new GameConfiguration(5, 5, 2, 3, 0, forceOrderlyTurn: 0), 1, BoardState.GameStateEnum.End, 1,
                expectedRemainingPlayers: new List<int>() { 1 })
        };
    }
}
