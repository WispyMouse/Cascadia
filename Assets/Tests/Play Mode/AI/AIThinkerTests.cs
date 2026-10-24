namespace Cascadia.PlayModeTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cascadia.DataStructures;
    using Cascadia.TestData;
    using Cascadia.Unity;
    using Cascadia.Unity.UI;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using static Cascadia.TestData.GameResultTestData;
    using static Cascadia.TestData.TestUtilities;

    /// <summary>
    /// Runs the <see cref="PlayModeSmokeTests"/> involving play, but replaces random members with AI.
    /// The goal is to ensure that the AIThinker Loop is acting appropriately.
    /// This will iterate over all possible permutations of AI involvement, to ensure that this works in all realities.
    /// </summary>
    public class AIThinkerTests
    {
        public readonly struct AITest_CoordinatesToPlay_DataSource_Object
        {
            public readonly GameResultTestData.CoordinatesToPlay_DataSource_Object BaseData;
            public readonly IEnumerable<int> AICoreSides;

            public AITest_CoordinatesToPlay_DataSource_Object(IEnumerable<int> aiCoreSides, GameResultTestData.CoordinatesToPlay_DataSource_Object baseData)
            {
                this.AICoreSides = aiCoreSides;
                this.BaseData = baseData;
            }

            public override string ToString()
            {
                string aiCoresString = string.Join(", ", this.AICoreSides);
                return $"(AIs are on Indexes {aiCoresString}) {this.BaseData}";
            }
        }

        public static AITest_CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource
        {
            get
            {
                List<AITest_CoordinatesToPlay_DataSource_Object> coordinates = new();

                foreach (CoordinatesToPlay_DataSource_Object data in GameResultTestData.CoordinatesToPlay_DataSource)
                {
                    // For each permutation of missing place, add it to the list
                    foreach (IEnumerable<int> permutation in GameResultTestData.FastPowerSet(data.GameConfiguration.PlayerCount))
                    {
                        coordinates.Add(new(permutation, data));
                    }
                }

                return coordinates.ToArray();
            }
        }

        [UnityTest]
        public IEnumerator ResultChecks([ValueSource(nameof(CoordinatesToPlay_DataSource))] AITest_CoordinatesToPlay_DataSource_Object plan)
        {
            yield return PlayModeTestUtility.LoadStartScene();

            PersistentGameConfiguration config = Object.FindFirstObjectByType<PersistentGameConfiguration>();
            config.HumanitizeForTests();
            config.ForceSetConfig(plan.BaseData.GameConfiguration);
            config.AnimationSpeedModifier = .05f;

            for (int ii = 0; ii < plan.BaseData.GameConfiguration.PlayerCount; ii++)
            {
                if (!plan.AICoreSides.Contains(ii))
                {
                    continue;
                }

                List<Coordinate> coordinatesToPlay = new();

                // This side should be AI; notate each of their plays
                foreach (CoordinateBySide coordinate in plan.BaseData.Coordinates)
                {
                    if (coordinate.Side == ii)
                    {
                        coordinatesToPlay.Add(coordinate.Placement);
                    }
                }

                TestExactChoiceAICore core = ScriptableObject.CreateInstance<TestExactChoiceAICore>().SetPlays(coordinatesToPlay);
                config.Players[ii].AICoreIndex = config.AddAICore(core);
            }

            yield return PlayModeTestUtility.LoadGameplayScene();

            GameConductor gameConductor = Object.FindFirstObjectByType<GameConductor>();
            CurrentTurnPanelEntry[] turnEntries = GameObject.FindObjectsByType<CurrentTurnPanelEntry>(FindObjectsSortMode.None);

            for (int ii = 0; ii < plan.BaseData.Coordinates.Count; ii++)
            {
                // Track how many plays have been made so we're moving *exactly* as expected,
                // otherwise the AI might make multiple turns
                int expectedActionCount = gameConductor.CurrentBoardState.MoveCommandsApplied.Count + 1;
                CoordinateBySide coordinateBySide = plan.BaseData.Coordinates[ii];

                // Ensure the UI thinks it is this player's turn
                Assert.AreEqual(coordinateBySide.Side, gameConductor.CurrentBoardState.CurrentPlayerTurnIndex, $"Move {ii}, Expecting side {coordinateBySide.Side} to be placing this turn");
                foreach (CurrentTurnPanelEntry currentTurn in turnEntries)
                {
                    bool shouldBeActive = currentTurn.RepresentedProfile.Index == coordinateBySide.Side;
                    Assert.AreEqual(shouldBeActive, currentTurn.IsCurrentTurnIndicator.activeInHierarchy, $"Move {ii}, Expecting the current turn indicator to correctly indicate current turn status of {coordinateBySide.Side} ({shouldBeActive})");
                }

                // If it is an AI's turn, then they should already be aware that the must act
                // Wait briefly until it becomes the next player's turn.
                // If it doesn't become the next's player turn after a second of waiting,
                // then the AI must have gotten stuck
                if (config.Players[coordinateBySide.Side].IsAI)
                {
                    float waitingTimeLeft = 1f;
                    bool turnTaken = false;

                    void markTurnTaken(PlayerSide playerSide)
                    {
                        Assert.AreEqual(coordinateBySide.Side, playerSide.PlayerProfileIndex, $"Move {ii}, The expected move should have acted");
                        turnTaken = true;
                    }

                    gameConductor.CurrentBoardState.BoardStateEvents.OnPlayerMadeMove += markTurnTaken;

                    while (!turnTaken && waitingTimeLeft > 0)
                    {
                        yield return null;
                        waitingTimeLeft -= Time.deltaTime;
                    }

                    if (!turnTaken)
                    {
                        Assert.Fail($"Move {ii}, AI didn't progress the turn state as expected. It is still their turn to move.");
                    }

                    gameConductor.CurrentBoardState.BoardStateEvents.OnPlayerMadeMove -= markTurnTaken;
                }
                else
                {
                    // Otherwise, manually choose it
                    gameConductor.ChooseCell(coordinateBySide.Placement);
                }

                // The most recently chosen cell should be what we told it to be
                Assert.AreEqual(coordinateBySide.Placement, gameConductor.CurrentBoardState.LastPlay?.FromCommand.Position, $"Move {ii}, Expecting the most recent coordinate played to be {coordinateBySide.Placement}. Was instead {gameConductor.CurrentBoardState.LastPlay?.FromCommand.Position}.");
                Assert.AreEqual(expectedActionCount, gameConductor.CurrentBoardState.MoveCommandsApplied.Count, $"Move {ii}, Expecting that a specific number of actions has been applied. Either it isn't applying or too many moves are being applied in one step. Is the AI taking extra actions?");
            }

            Assert.AreEqual(BoardState.GameStateEnum.End, gameConductor.CurrentBoardState.CurrentGameState, $"Expecting board state to result in the end of a game.");
            Assert.AreEqual(plan.BaseData.Winner, gameConductor.CurrentBoardState.Winner?.PlayerProfileIndex, $"Expecting a predicted winner / null lack of winner.");
        }
    }
}
