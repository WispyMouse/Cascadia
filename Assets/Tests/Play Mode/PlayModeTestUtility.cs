namespace Cascadia.PlayModeTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Cascadia.DataStructures;
    using Cascadia.Unity;
    using Cascadia.Unity.UI;
    using Cascadia.Unity.UI.Reusable;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Composites;
    using UnityEngine.InputSystem.Controls;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

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

            // HACK: We want all of the player's icons to be loaded before continuing
            // Ideally, we shouldn't be wasting processing power by loading tons of real icons!
            // For now, patiently wait to reduce errors; it matches human reaction times anyways
            while (PersistentGameConfiguration.Singleton.Players.Any(x => x.Sprite.NeedsResolution))
            {
                yield return null;
            }

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

            if (!associatedGameObject.TryGetComponent<Button>(out Button buttonComponent))
            {
                Assert.Fail($"{buttonFindableName} was found but it was not attached to a Button.");
            }

            yield return ClickOnButton(buttonComponent);
        }

        public static IEnumerator ClickOnButton(Button button)
        {
            if (button == null)
            {
                Assert.Fail($"Provided button was null.");
            }

            if (!button.gameObject.activeInHierarchy)
            {
                Assert.Fail($"Specified button '{button.name}' should not be inactive. [{SpoolOutTransformHierarchy(button.transform)}]");
            }

            if (!button.gameObject.TryGetComponent<RectTransform>(out RectTransform transformWithName))
            {
                Assert.Fail($"Found button with name '{button.name}', but it didn't have a RectTransform.");
            }

            // Since we know this click is for a Button, we can attach
            // event listeners to make double sure that this button
            // considered itself to have been clicked
            bool onClickRun = false;

            void OnClickRunMethod()
            {
                onClickRun = true;
            }

            button.onClick.AddListener(OnClickRunMethod);

            yield return ClickOn(transformWithName);

            button.onClick.RemoveListener(OnClickRunMethod);

            Assert.IsTrue(onClickRun, $"Expected that the onclick method would run.");
        }

        public static IEnumerator ClickOn(RectTransform transform)
        {
            yield return null;

            Mouse currentMouse = InputSystem.AddDevice<Mouse>();

            // Move mouse over object
            using (StateEvent.From(currentMouse, out InputEventPtr ptr))
            {
                Vector2 center = GetScreenCoordinates(transform).center;

                if (center.x < 0 || center.y < 0 || center.x > Screen.width || center.y > Screen.height)
                {
                    Assert.Fail($"Asked to click outside of the screen to click on {transform.gameObject.name} [{SpoolOutTransformHierarchy(transform)}] @ {center} (max size is {Screen.width}x{Screen.height}");
                }

                // Make sure the top element is this
                GameObject clickedObject = GetUIElementAtScreenPosition(center);
                Assert.AreEqual(transform.gameObject, clickedObject, $"Expecting that the top ui element at position is {transform.gameObject.name} [{SpoolOutTransformHierarchy(transform)}]. Was instead {clickedObject?.name}");

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
            PointerEventData pointerData = new(EventSystem.current)
            {
                position = screenPosition
            };

            List<RaycastResult> results = new();

            if (EventSystem.current != null)
            {
                EventSystem.current.RaycastAll(pointerData, results);
            }

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

        public static string SpoolOutTransformHierarchy(Transform toSpool)
        {
            StringBuilder sb = new();

            Transform next = toSpool;
            while (next != null)
            {
                sb.Append($"=> {next.name}");

                next = next.parent;
            }

            return sb.ToString();
        }
    }
}
