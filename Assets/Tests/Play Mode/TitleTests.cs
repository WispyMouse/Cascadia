namespace Cascadia.PlayModeTests
{
    using System.Collections;
    using Cascadia.Unity.UI;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.UIElements;

    public class TitleTests
    {
        [UnityTest]
        public IEnumerator CanLoadTitle()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();
            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.TITLE);
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
            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.RULESPAGEONE);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESFROMONETOTWOBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.RULESPAGETWO);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESFROMTWOTOONEBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.RULESPAGEONE);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESFROMONETOTWOBUTTONNAME);
            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESRETURNBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.TITLE);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.RULESPAGEONE);
        }

        [UnityTest]
        public IEnumerator PressStartOnTitle()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();

            yield return PlayModeTestUtility.ClickOnButton(Findable.GAMESETUPSTARTBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.GAMESETUP);

            yield return PlayModeTestUtility.ClickOnButton(Findable.GAMESETUPBACKBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.TITLE);
        }

        [UnityTest]
        public IEnumerator SelectBoardScreenOpens()
        {
            yield return PlayModeTestUtility.LoadInToTitleScene();

            yield return PlayModeTestUtility.ClickOnButton(Findable.GAMESETUPSTARTBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.GAMESETUP);

            yield return PlayModeTestUtility.ClickOnButton(Findable.SELECTBOARDBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.SELECTBOARD);

            yield return PlayModeTestUtility.ClickOnButton(Findable.SELECTBOARDBACKBUTTONNAME);

            PlayModeTestUtility.AssertTitleMenuOpenness(Findable.TITLE.CANVAS.GAMESETUP);
        }
    }
}
