namespace Cascadia.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.DataStructures;
    using Cascadia.TestData;
    using System.Runtime.CompilerServices;
    using static Cascadia.TestData.UndoTestsData;

    public class UndoTests
    {
        public static UndoTestsData.UndoTests_UndosAtEnd_DataSource_Object[] UndoTests_UndosAtEnd_DataSource => UndoTestsData.UndoTests_UndosAtEnd_DataSource;

        [Test]
        [TestCaseSource(nameof(UndoTests_UndosAtEnd_DataSource))]
        public void UndoSidesAsExpected(UndoTests_UndosAtEnd_DataSource_Object plan)
        {
            BoardState testState = new(plan.GameConfiguration);
            List<PlayerSide> expectedOwnership = new();

            for (int ii = 0, count = plan.Placements.Count; ii < count; ii++)
            {
                testState.ApplyMoveCommand(new MoveCommand(testState.CurrentPlayerSide, plan.Placements[ii].Placement), true);
                expectedOwnership.Add(testState.GetCoordinateOwnership(plan.Placements[ii].Placement));
            }

            for (int ii = 0; ii < plan.UndosAtEnd; ii++)
            {
                testState.ReversePreviousMoveCommand();
            }

            Assert.AreEqual(plan.ExpectedGameStateEnum, testState.CurrentGameState, $"Expecting game state to be in specific status");
            Assert.AreEqual(plan.ExpectedPlayerIndex, testState.CurrentPlayerTurnIndex, $"Expecting current player index to be specific");
            Assert.AreEqual(plan.ExpectedCascade, testState.CurrentCascadeLevel, $"Expecting cascade to be specific");
            Assert.AreEqual(plan.ExpectedRemainingPlayers, testState.PlayerSidesStillInGame.Count, $"Expecting remaining player count to be specific");

            for (int ii = 0, count = plan.Placements.Count - plan.UndosAtEnd; ii < count; ii++)
            {
                Assert.IsTrue(testState.GetCoordinateOwnership(plan.Placements[ii].Placement) == expectedOwnership[ii], "Expecting ownership to still be claimed");
            }

            for (int ii = plan.Placements.Count - plan.UndosAtEnd + 1, count = plan.Placements.Count; ii < count; ii++)
            {
                Assert.IsTrue(testState.GetCoordinateOwnership(plan.Placements[ii].Placement) == null, "Expecting ownership to now be empty");
            }
        }
    }
}
