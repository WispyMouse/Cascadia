using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cascadia.DataStructures;
using Cascadia.PlayModeTests;
using Cascadia.TestData;
using Cascadia.Unity;
using Cascadia.Unity.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static Cascadia.TestData.TestUtilities;

public class PlayModeSmokeTests
{
    public static GameResultTestData.CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource => GameResultTestData.CoordinatesToPlay_DataSource;
    public static KnockoutTestsData.Knockout_ExpectedPlayer_DataSource_Object[] KnockoutToPlay_DataSource => KnockoutTestsData.Knockout_ExpectedPlayer_DataSource;
    public static IEnumerable<int> PlayerCounts_DataSource => TestUtilities.PlayerCountConfigurations;

    /// <summary>
    /// Are we able to navigate to the gameplay scene without errors being printed?
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator LoadGameplayWithoutError()
    {
        yield return PlayModeTestUtility.LoadStartScene();
        yield return PlayModeTestUtility.LoadGameplayScene();
        Assert.Pass();
    }

    /// <summary>
    /// Loads up the play scene with the specified number of players.
    /// Ensures their sprites are set as expected, and that they've loaded in completely.
    /// </summary>
    [UnityTest]
    public IEnumerator PlaySceneUsesCorrectSprites([ValueSource(nameof(PlayerCounts_DataSource))] int playerCount)
    {
        yield return PlayModeTestUtility.LoadStartScene();

        PersistentGameConfiguration config = Object.FindFirstObjectByType<PersistentGameConfiguration>();
        config.HumanitizeForTests();
        config.ForceSetConfig(new GameConfiguration(5, 5, playerCount, 3, 0, forceOrderlyTurn: 0));
        Assert.AreEqual(playerCount, config.GameConfiguration.PlayerCount, $"Expecting specific number of players.");

        yield return PlayModeTestUtility.LoadGameplayScene();
        CurrentTurnPanel turnPanel = GameObject.FindFirstObjectByType<CurrentTurnPanel>();

        Assert.AreEqual(config.GameConfiguration.PlayerCount, turnPanel.Entries.Count, $"Expecting specific number of players.")
;
        for (int ii = 0; ii < turnPanel.Entries.Count; ii++)
        {
            Assert.AreEqual(config.Players[ii].Sprite.Sprite, turnPanel.Entries[ii].PlayerIcon.sprite, $"Expecting the player to have the assigned sprite.");
        }
    }

    [UnityTest]
    public IEnumerator ResultChecks([ValueSource(nameof(CoordinatesToPlay_DataSource))] GameResultTestData.CoordinatesToPlay_DataSource_Object plan)
    {
        yield return PlayModeTestUtility.LoadStartScene();

        PersistentGameConfiguration config = Object.FindFirstObjectByType<PersistentGameConfiguration>();
        config.HumanitizeForTests();
        config.ForceSetConfig(plan.GameConfiguration);

        yield return PlayModeTestUtility.LoadGameplayScene();

        GameConductor gameConductor = Object.FindFirstObjectByType<GameConductor>();

        for (int ii = 0; ii < plan.Coordinates.Count; ii++)
        {
            CoordinateBySide coordinateBySide = plan.Coordinates[ii];
            Assert.AreEqual(coordinateBySide.Side, gameConductor.CurrentBoardState.CurrentPlayerTurnIndex, $"Expecting side {coordinateBySide.Side} to be placing this turn");
            gameConductor.ChooseCell(coordinateBySide.Placement);
        }

        Assert.AreEqual(BoardState.GameStateEnum.End, gameConductor.CurrentBoardState.CurrentGameState, $"Expecting board state to result in the end of a game.");
        Assert.AreEqual(plan.Winner, gameConductor.CurrentBoardState.Winner?.PlayerProfileIndex, $"Expecting a predicted winner / null lack of winner.");
    }

    [UnityTest]
    public IEnumerator KnockoutChecks([ValueSource(nameof(KnockoutToPlay_DataSource))] KnockoutTestsData.Knockout_ExpectedPlayer_DataSource_Object plan)
    {
        yield return PlayModeTestUtility.LoadStartScene();

        PersistentGameConfiguration config = Object.FindFirstObjectByType<PersistentGameConfiguration>();
        config.HumanitizeForTests();
        config.ForceSetConfig(plan.GameConfiguration);

        yield return PlayModeTestUtility.LoadGameplayScene();

        CurrentTurnPanel turnPanel = GameObject.FindFirstObjectByType<CurrentTurnPanel>();
        GameConductor gameConductor = Object.FindFirstObjectByType<GameConductor>();

        for (int ii = 0; ii < plan.Placements.Count; ii++)
        {
            CoordinateBySide coordinateBySide = plan.Placements[ii];
            Assert.AreEqual(coordinateBySide.Side, gameConductor.CurrentBoardState.CurrentPlayerTurnIndex, $"Expecting side {coordinateBySide.Side} to be placing this turn");
            gameConductor.ChooseCell(coordinateBySide.Placement);
        }

        Assert.AreEqual(plan.ExpectedGameStateEnum, gameConductor.CurrentBoardState.CurrentGameState, $"Expecting game state to be in specific status");
        Assert.AreEqual(plan.ExpectedPlayerIndex, gameConductor.CurrentBoardState.CurrentPlayerTurnIndex, $"Expecting current player index to be specific");
        Assert.AreEqual(plan.ExpectedCascade, gameConductor.CurrentBoardState.CurrentCascadeLevel, $"Expecting cascade to be specific");
        Assert.AreEqual(plan.ExpectedRemainingPlayers.Count, gameConductor.CurrentBoardState.PlayerSidesStillInGame.Count, $"Expecting remaining player count to be specific");

        // Did the map mark as expected?
        Assert.NotNull(turnPanel);
        foreach (CurrentTurnPanelEntry entry in turnPanel.Entries)
        {
            bool shouldBeKnockedOut = !plan.ExpectedRemainingPlayers.Contains(entry.RepresentedSide.PlayerProfileIndex);
            Assert.AreEqual(shouldBeKnockedOut,
                entry.IsPlayerSideKnockedOut,
                $"Expected the knockout panel to reflect current knockout status. ({shouldBeKnockedOut})");
        }
    }
}
