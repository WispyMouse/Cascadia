namespace Cascadia.Tests
{
    using NUnit.Framework;
    using Cascadia.DataStructures;
    using Cascadia.TestData;

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
                Assert.IsTrue(testState.TryApplyMoveCommand(new MoveCommand(testState.CurrentPlayerIndex, plan.Placements[ii]), true));
            }

            Assert.AreEqual(plan.ExpectedGameStateEnum, testState.CurrentGameState, $"Expecting game state to be in specific status");
            Assert.AreEqual(plan.ExpectedPlayerIndex, testState.CurrentPlayerIndex, $"Expecting current player index to be specific");
            Assert.AreEqual(plan.ExpectedCascade, testState.CurrentCascadeLevel, $"Expecting cascade to be specific");
            Assert.AreEqual(plan.ExpectedRemainingPlayers, testState.SidesStillInGame.Count, $"Expecting remaining player count to be specific");
        }
    }

}
