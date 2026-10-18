using System.Collections;
using Cascadia.DataStructures;
using Cascadia.PlayModeTests;
using Cascadia.TestData;
using Cascadia.Unity;
using Cascadia.Unity.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayModeSmokeTests
{
    public static GameResultTestData.CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource => GameResultTestData.CoordinatesToPlay_DataSource;
    public static KnockoutTestsData.Knockout_ExpectedPlayer_DataSource_Object[] KnockoutToPlay_DataSource => KnockoutTestsData.Knockout_ExpectedPlayer_DataSource;

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
            gameConductor.ChooseCell(plan.Coordinates[ii]);
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

        GameConductor gameConductor = Object.FindFirstObjectByType<GameConductor>();

        for (int ii = 0; ii < plan.Placements.Count; ii++)
        {
            gameConductor.ChooseCell(plan.Placements[ii]);
        }

        Assert.AreEqual(plan.ExpectedGameStateEnum, gameConductor.CurrentBoardState.CurrentGameState, $"Expecting game state to be in specific status");
        Assert.AreEqual(plan.ExpectedPlayerIndex, gameConductor.CurrentBoardState.CurrentPlayerTurnIndex, $"Expecting current player index to be specific");
        Assert.AreEqual(plan.ExpectedCascade, gameConductor.CurrentBoardState.CurrentCascadeLevel, $"Expecting cascade to be specific");
        Assert.AreEqual(plan.ExpectedRemainingPlayers, gameConductor.CurrentBoardState.PlayerSidesStillInGame.Count, $"Expecting remaining player count to be specific");
    }
}
