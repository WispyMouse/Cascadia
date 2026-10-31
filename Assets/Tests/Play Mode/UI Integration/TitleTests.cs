namespace Cascadia.PlayModeTests
{
    using System.Collections;
    using System.Linq;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI;
    using Cascadia.Unity.UI.GameScene;
    using Cascadia.Unity.UI.Reusable;
    using Cascadia.Unity.UI.SetupScene;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.UI;
    using static UnityEngine.ParticleSystem;

    public class TitleTests
    {
        [UnityTest]
        public IEnumerator CanLoadTitle()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();
            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.TITLE);
        }

        /// <summary>
        /// Start at the title screen. Open the rules screen.
        /// Click forward and backward.
        /// Close on page 2. Re-open.
        /// Pages should be open or closed as expected.
        /// </summary>
        [UnityTest]
        public IEnumerator RulesNavigation()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.RULESBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.RULESPAGEONE);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.RULESFROMONETOTWOBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.RULESPAGETWO);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.RULESFROMTWOTOONEBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.RULESPAGEONE);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.RULESFROMONETOTWOBUTTONNAME);
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.RULESRETURNBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.TITLE);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.RULESBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.RULESPAGEONE);
        }

        [UnityTest]
        public IEnumerator PressStartOnTitle()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.GAMESETUPSTARTBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.GAMESETUP);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.GAMESETUPBACKBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.TITLE);
        }

        [UnityTest]
        public IEnumerator SelectBoardScreenOpens()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.GAMESETUPSTARTBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.GAMESETUP);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.SELECTBOARDBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.SELECTBOARD);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.SELECTBOARDBACKBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.GAMESETUP);
        }

        [UnityTest]
        public IEnumerator ConfigurePlayersOpens()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.GAMESETUPSTARTBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.GAMESETUP);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.CONFIGUREPLAYERSBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.CONFIGUREPLAYERS);

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.CONFIGUREPLAYERSBACKBUTTON);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.GAMESETUP);

            // Click back in to the configure players area
            // This is meant to catch obvious errors from navigating back in to this menu
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.CONFIGUREPLAYERSBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.CONFIGUREPLAYERS);
        }

        [UnityTest]
        public IEnumerator EditPlayerOpens()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.GAMESETUPSTARTBUTTONNAME);
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.CONFIGUREPLAYERSBUTTONNAME);
            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.CONFIGUREPLAYERS);

            PlayerConfiguratorSelector playerConfiguratorSelector = GameObject.FindFirstObjectByType<PlayerConfiguratorSelector>();
            PlayerOperationsButton[] playerOperationsButtons = GameObject.FindObjectsByType<PlayerOperationsButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            playerOperationsButtons = playerOperationsButtons.OrderBy(x => x.transform.GetSiblingIndex()).ToArray();

            Assert.IsNotEmpty(playerOperationsButtons, $"Expecting there be to be visible, active edit player buttons");

            for (int ii = 0; ii < BoardState.MAXIMUMPLAYERS; ii++)
            {
                PlayerOperationsButton currentPlayerOperations = playerOperationsButtons[ii];
                Button plusButton = currentPlayerOperations.PlusButton;

                // If this is after the first buttons, we'll need to make it active for it to have the edit butotn
                if (ii >= BoardState.MINIMUMPLAYERS)
                {
                    Assert.NotNull(plusButton, $"Expecting to find a child named Plus Button at a specific path");
                    yield return PlayModeTestUtility.ClickOn(plusButton.GetComponent<RectTransform>());
                    Assert.AreEqual(ii + 1, playerConfiguratorSelector.CurrentlyConfigurablePlayerButtonsCount, $"Expecting a specific amount of visible player buttons");
                }
                else
                {
                    Assert.AreEqual(BoardState.MINIMUMPLAYERS, playerConfiguratorSelector.CurrentlyConfigurablePlayerButtonsCount, $"Before expanding the first unexpanded button, there should be {BoardState.MINIMUMPLAYERS} players");
                }

                Assert.IsFalse(plusButton.gameObject.activeInHierarchy, $"Expecting plus button to be hidden now");

                Button configureTransform = currentPlayerOperations.ConfigureButton;

                yield return PlayModeTestUtility.ClickOn(configureTransform.GetComponent<RectTransform>());
                PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.EDITPLAYER);
                yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.EDITPLAYERBACKBUTTON);
                PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.CONFIGUREPLAYERS);
            }

            // Minus everything and ensure they're removed
            for (int ii = 5; ii >= BoardState.MINIMUMPLAYERS; ii--)
            {
                PlayerOperationsButton findButton = playerOperationsButtons[ii];

                Transform configureTransform = findButton.transform.Find("OperationsButtons/Minus/Minus Button");
                Assert.NotNull(configureTransform, $"Expecting to find a child named Minus Button at a specific path");
                yield return PlayModeTestUtility.ClickOn(configureTransform.GetComponent<RectTransform>());
                Assert.AreEqual(ii, playerConfiguratorSelector.CurrentlyConfigurablePlayerButtonsCount, $"Expecting a specific amount of visible player buttons after minus-ing");

                Transform plusButton = findButton.transform.Find("Icon Representer/Plus/Plus Button");
                Assert.IsTrue(plusButton.gameObject.activeInHierarchy, $"Expecting that the plus button will be visible now");
            }
        }

        [UnityTest]
        public IEnumerator CanPickCategoryAndIcon()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.GAMESETUPSTARTBUTTONNAME);
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.CONFIGUREPLAYERSBUTTONNAME);
            PlayerConfiguratorSelector playerConfiguratorSelector = GameObject.FindObjectsByType<PlayerConfiguratorSelector>(FindObjectsSortMode.None)[0];
            PlayerOperationsButton playerOperationsButton = GameObject.FindObjectsByType<PlayerOperationsButton>(FindObjectsSortMode.None).OrderBy(x => x.transform.GetSiblingIndex()).First();
            yield return PlayModeTestUtility.ClickOnButton(playerOperationsButton.ConfigureButton);
            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.EDITPLAYER);
            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.SELECTEMOJIEDITBUTTON);
            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.CHOOSEEMOJI);

            // What category is currently showing?
            ChooseEmojiCanvas canvas = GameObject.FindFirstObjectByType<ChooseEmojiCanvas>();
            string currentCategory = canvas.SelectedCategory;

            // Wait for the screen to populate
            yield return null;

            // We want to press a button for some other category in the list; we will definitely achieve that
            // if we click on *two* buttons and check between
            ChooseEmojiCategory[] categories = GameObject.FindObjectsByType<ChooseEmojiCategory>(FindObjectsSortMode.None).OrderBy(x => x.transform.GetSiblingIndex()).ToArray();

            // Assuming at least two categories here
            ChooseEmojiCategory category = categories[0];

            if (category.RepresentedCategory.Equals(currentCategory))
            {
                if (categories.Length <= 1)
                {
                    Assert.Fail($"There aren't enough categories to choose from for this test. There should at least be two categories with different category labels.");
                }

                // When selecting a random emoji, it must have selected the category of the previous button
                // so get this next button
                category = categories[1];
            }

            yield return PlayModeTestUtility.ClickOnButton(category.Button);

            // Wait a moment for everything to load in; the buttons are asynchronously loaded
            // HACK TODO: This should definitely be a more intelligent wait, can we hook it up to something?
            yield return new WaitForSeconds(.6f);

            // We must be in a new category, so clicking on any of these should change it
            Assert.AreEqual(category.RepresentedCategory, canvas.SelectedCategory, $"Expecting that pressing a category button would change the category");
            ChooseEmojiIcon iconButton = GameObject.FindObjectsByType<ChooseEmojiIcon>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).OrderBy(x => x.transform.GetSiblingIndex()).First();
            yield return PlayModeTestUtility.ClickOnButton(iconButton.Button);

            Assert.IsTrue(canvas.ShowingForProfile.MatchesSprite(iconButton.RepresentedEmoji), $"Expecting that the sprite has been set.");

            yield return PlayModeTestUtility.ClickOnButton(Findable.TITLE.BUTTONNAMES.CHOOSEEMOJIBACKBUTTON);
            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVASNAMES.EDITPLAYER);

            EditPlayerScreen editPlayerScreen = GameObject.FindFirstObjectByType<EditPlayerScreen>();
            Assert.AreEqual(iconButton.RepresentedEmoji, editPlayerScreen.CurrentlyDisplayedPlayerSprite, $"Expecting the new sprite to be represented in the UI.");

            // TODO: Make sure it updates the general player setup screen
        }
    }
}
