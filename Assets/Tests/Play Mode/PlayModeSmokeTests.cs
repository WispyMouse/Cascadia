using System.Collections;
using NUnit.Framework;
using TicTacCOSTCO.DataStructures;
using TicTacCOSTCO.Unity;
using TicTacCOSTCO.Unity.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayModeSmokeTests
{
    public const string TestingStartScene = "TestingStartScene";
    public const string GameplayScene = "GameplayScene";

    public static ResultsTestsData.CoordinatesToPlay_DataSource_Object[] CoordinatesToPlay_DataSource => ResultsTestsData.CoordinatesToPlay_DataSource;

    /// <summary>
    /// Are we able to navigate to the gameplay scene without errors being printed?
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator LoadGameplayWithoutError()
    {
        yield return LoadStartScene();
        yield return LoadGameplayScene();
        Assert.Pass();
    }

    private IEnumerator LoadStartScene()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(TestingStartScene);
        while (!loadOp.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadGameplayScene()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(GameplayScene);
        while (!loadOp.isDone)
        {
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator ResultChecks([ValueSource(nameof(CoordinatesToPlay_DataSource))] ResultsTestsData.CoordinatesToPlay_DataSource_Object plan)
    {
        yield return LoadStartScene();

        PersistentGameConfiguration config = GameObject.FindFirstObjectByType<PersistentGameConfiguration>();
        config.ForceSetConfig(plan.GameConfiguration);
        config.HumanitizeForTests();

        yield return LoadGameplayScene();

        GameConductor gameConductor = GameObject.FindFirstObjectByType<GameConductor>();

        for (int ii = 0; ii < plan.Coordinates.Count; ii++)
        {
            gameConductor.ChooseCell(plan.Coordinates[ii]);
        }

        Assert.AreEqual(BoardState.GameStateEnum.End, gameConductor.CurrentBoardState.CurrentGameState, $"Expecting board state to result in the end of a game.");
        Assert.AreEqual(plan.Winner, gameConductor.CurrentBoardState.Winner, $"Expecting a predicted winner / null lack of winner.");
    }
}
