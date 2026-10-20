namespace Cascadia.Tests
{
    using NUnit.Framework;
    using Cascadia.DataStructures;
    using Cascadia.TestData;
    using static Cascadia.TestData.TestUtilities;
    using UnityEditor.VersionControl;
    using System.Linq;

    public class KnockoutTests
    {
        public static KnockoutTestsData.Knockout_ExpectedPlayer_DataSource_Object[] CoordinatesToPlay_DataSource => KnockoutTestsData.Knockout_ExpectedPlayer_DataSource;

        [Test]
        [TestCaseSource(nameof(CoordinatesToPlay_DataSource))]
        public void KnockoutStatusAsExpected(KnockoutTestsData.Knockout_ExpectedPlayer_DataSource_Object plan)
        {
            BoardState testState = new(plan.GameConfiguration);

            for (int ii = 0, count = plan.Placements.Count; ii < count; ii++)
            {
                CoordinateBySide coordinateBySide = plan.Placements[ii];
                Assert.AreEqual(coordinateBySide.Side, testState.CurrentPlayerTurnIndex, $"Expecting side {coordinateBySide.Side} to be placing this turn");
                testState.ApplyMoveCommand(new MoveCommand(testState.CurrentPlayerSide, coordinateBySide.Placement), true);
            }

            Assert.AreEqual(plan.ExpectedGameStateEnum, testState.CurrentGameState, $"Expecting game state to be in specific status");
            Assert.AreEqual(plan.ExpectedPlayerIndex, testState.CurrentPlayerTurnIndex, $"Expecting current player index to be specific");
            Assert.AreEqual(plan.ExpectedCascade, testState.CurrentCascadeLevel, $"Expecting cascade to be specific");
            Assert.AreEqual(plan.ExpectedRemainingPlayers.Count, testState.PlayerSidesStillInGame.Count, $"Expecting remaining player count to be specific");

            for (int ii = 0, count = plan.GameConfiguration.PlayerCount; ii < count; ii++)
            {
                bool shouldBeInGame = plan.ExpectedRemainingPlayers.Contains(ii);
                Assert.AreEqual(shouldBeInGame ? 1 : 0, testState.PlayerSidesStillInGame.Count(x => x.TurnOrderIndex == ii),
                    $"Expecting player to be in game based on plan {ii} {shouldBeInGame}");
            }
        }
    }

}
