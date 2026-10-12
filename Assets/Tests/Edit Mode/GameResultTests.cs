using NUnit.Framework;
using System.Collections.Generic;
using Cascadia.DataStructures;
using UnityEngine;
using static Cascadia.Tests.CoordinateTests;

namespace Cascadia.Tests
{
    public class GameResultTests
    {
        public static ResultsTestsData.CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource => ResultsTestsData.CoordinatesToPlay_DataSource;

        [Test]
        [TestCaseSource(nameof(CoordinatesToPlay_DataSource))]
        public void ResultChecks(ResultsTestsData.CoordinatesToPlay_DataSource_Object plan)
        {
            BoardState holder = new BoardState(plan.GameConfiguration);

            for (int ii = 0; ii < plan.Coordinates.Count; ii++)
            {
                Assert.IsTrue(holder.TryApplyMoveCommand(holder.GenerateCommandFromMove(holder.CurrentPlayerSide, plan.Coordinates[ii])));
            }

            Assert.AreEqual(BoardState.GameStateEnum.End, holder.CurrentGameState, $"Expecting board state to result in the end of a game.");
            Assert.AreEqual(plan.Winner, holder.Winner, $"Expecting a predicted winner / null lack of winner.");
        }
    }
}
