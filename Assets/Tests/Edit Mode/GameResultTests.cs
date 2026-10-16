namespace Cascadia.EditModeTests
{
    using NUnit.Framework;
    using Cascadia.DataStructures;
    using Cascadia.TestData;

    public class GameResultTests
    {
        public static GameResultTestData.CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource => GameResultTestData.CoordinatesToPlay_DataSource;

        [Test]
        [TestCaseSource(nameof(CoordinatesToPlay_DataSource))]
        public void ResultChecks(GameResultTestData.CoordinatesToPlay_DataSource_Object plan)
        {
            BoardState holder = new(plan.GameConfiguration);

            for (int ii = 0; ii < plan.Coordinates.Count; ii++)
            {
                holder.ApplyMoveCommand(new MoveCommand(holder.CurrentPlayerSide, plan.Coordinates[ii]));
            }

            Assert.AreEqual(BoardState.GameStateEnum.End, holder.CurrentGameState, $"Expecting board state to result in the end of a game.");
            Assert.AreEqual(plan.Winner, holder.Winner?.PlayerProfileIndex, $"Expecting a predicted winner / null lack of winner.");
        }
    }
}
