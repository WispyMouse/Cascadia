namespace Cascadia.PlayModeTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.Unity.UI;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Composites;
    using UnityEngine.InputSystem.Controls;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.SceneManagement;

    public class PlayModeTestUtility
    {
        public const string TESTINGSTARTSCENE = "TestingStartScene";
        public const string GAMEPLAYSCENE = "GameplayScene";
        public const string TITLESCENE = "TitleScene";

        public static IEnumerator LoadStartScene()
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(PlayModeTestUtility.TESTINGSTARTSCENE);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            yield return new WaitForFixedUpdate();
        }

        public static IEnumerator LoadInToTitleScene()
        {
            yield return LoadStartScene();

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(PlayModeTestUtility.TITLESCENE);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            yield return new WaitForFixedUpdate();
        }

        public static IEnumerator LoadGameplayScene()
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(PlayModeTestUtility.GAMEPLAYSCENE);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            yield return new WaitForFixedUpdate();
        }

        public static IEnumerator ClickOnButton(string buttonFindableName)
        {
            GameObject associatedGameObject = GameObject.Find(buttonFindableName);

            if (associatedGameObject == null)
            {
                Assert.Fail($"Could not find a button with findable name '{buttonFindableName}'.");
            }

            if (!associatedGameObject.activeInHierarchy)
            {
                Assert.Fail($"Specified button {buttonFindableName} should not be inactive.");
            }

            if (!associatedGameObject.TryGetComponent<RectTransform>(out RectTransform transformWithName))
            {
                Assert.Fail($"Found button with name '{buttonFindableName}', but it didn't have a RectTransform.");
            }

            yield return ClickOn(transformWithName);
        }

        public static IEnumerator ClickOn(RectTransform transform)
        {
            yield return null;

            Mouse currentMouse = InputSystem.AddDevice<Mouse>();

            // Move mouse over object
            using (StateEvent.From(currentMouse, out InputEventPtr ptr))
            {
                Vector2 center = GetScreenCoordinates(transform).center;

                // Make sure the top element is this
                GameObject clickedObject = GetUIElementAtScreenPosition(center);
                Assert.AreEqual(transform.gameObject, clickedObject, $"Expecting that the top ui element at position is {transform.gameObject.name}. Was instead {clickedObject?.name}");

                currentMouse.position.WriteValueIntoEvent<Vector2>(center, ptr);
                InputSystem.QueueEvent(ptr);
            }

            InputSystem.Update();

            // Wait one frame so that the mouse can move
            yield return null;

            // Wait a frame so that the mouse over events activate
            yield return null;

            // Click it
            using (StateEvent.From(currentMouse, out InputEventPtr ptr))
            {
                currentMouse.leftButton.WriteValueIntoEvent(1f, ptr);
                InputSystem.QueueEvent(ptr);
            }

            InputSystem.Update();
            Assert.AreEqual(true, currentMouse.leftButton.isPressed, $"Expecting that we're now pressing the left mouse button.");

            // Wait one frame with the mouse button down
            yield return null;

            // Release it
            using (StateEvent.From(currentMouse, out InputEventPtr ptr))
            {
                currentMouse.leftButton.WriteValueIntoEvent(0f, ptr);
                InputSystem.QueueEvent(ptr);
            }

            InputSystem.Update();
            Assert.AreEqual(false, currentMouse.leftButton.isPressed, $"Expecting that we've released the left mouse button.");

            // Wait once for the release event to be processed
            yield return null;

            // Wait for the release event to start processing
            yield return null;

            // Wait for the release event to have completed firing
            yield return null;

            // Wait for next visual frame
            yield return Awaitable.NextFrameAsync();

            InputSystem.RemoveDevice(currentMouse);
        }

        public static void AssertObjectIsActive(string nameOfObject, bool isActive = true)
        {
            Findable associatedGameObject = GameObject.FindObjectsByType<Findable>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(x => x.name == nameOfObject);

            if (associatedGameObject == null)
            {
                Assert.Fail($"Could not find a game object with findable name '{nameOfObject}'.");
            }

            Assert.IsTrue(associatedGameObject.gameObject.activeInHierarchy == isActive, $"Object {nameOfObject} expected to be active ({isActive})");
        }

        public static Rect GetScreenCoordinates(RectTransform uiElement)
        {
            Vector3[] worldCorners = new Vector3[4];
            uiElement.GetWorldCorners(worldCorners);
            Rect result = new(
                          worldCorners[0].x,
                          worldCorners[0].y,
                          worldCorners[2].x - worldCorners[0].x,
                          worldCorners[2].y - worldCorners[0].y);
            return result;
        }

        public static GameObject GetUIElementAtScreenPosition(Vector2 screenPosition)
        {
            // 1. Set up the Pointer Event Data with the screen position
            PointerEventData pointerData = new(EventSystem.current)
            {
                position = screenPosition
            };

            // 2. Create a list to store all raycast hits
            List<RaycastResult> results = new();

            // 3. Raycast against the UI using the current EventSystem
            if (EventSystem.current != null)
            {
                EventSystem.current.RaycastAll(pointerData, results);
            }

            // 4. Return the topmost UI GameObject if any were hit
            if (results.Count > 0)
            {
                return results[0].gameObject;
            }

            return null; // No UI element found at this position
        }

        /// <summary>
        /// Iterates through all expected title menus, and asserts that only the expected one is open currently.
        /// </summary>
        /// <param name="exceptFor">The sole exception that should be open.</param>
        public static void AssertTitleMenuOpenness(string exceptFor)
        {
            foreach (string titleCanvas in Findable.TITLE.CANVASNAMES.TITLEMENUWINDOWS)
            {
                AssertObjectIsActive(titleCanvas, titleCanvas == exceptFor);
            }
        }
    }
}
