namespace Cascadia.TestData
{
    using System.Collections.Generic;
    using Cascadia.AIs;
    using Cascadia.DataStructures;
    using NUnit.Framework;
    using UnityEngine;

    /// <summary>
    /// AICore that choses moves exactly as prescribed
    /// </summary>
    public class TestExactChoiceAICore : AICore
    {
        public List<Coordinate> CoordinatesToPlay;

        public TestExactChoiceAICore SetPlays(List<Coordinate> coordinatesToPlay)
        {
            this.CoordinatesToPlay = coordinatesToPlay;
            return this;
        }

        public override Coordinate DetermineMove(PlayerSide forSide, BoardState currentGameState)
        {
            if (this.CoordinatesToPlay.Count > 0)
            {
                Coordinate nextCoordinate = this.CoordinatesToPlay[0];
                this.CoordinatesToPlay.RemoveAt(0);
                Debug.Log($"Player side {forSide} playing prescribed move {nextCoordinate}");
                return nextCoordinate;
            }

            Assert.Fail($"Test AICore asked to make choice, but there are no more prescribed moves");

            return base.DetermineMove(forSide, currentGameState);
        }
    }
}
