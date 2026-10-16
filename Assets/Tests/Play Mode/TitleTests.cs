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
            yield return PlayModeTestUtility.LoadStartScene();
            yield return PlayModeTestUtility.LoadTitleScene();

            PlayModeTestUtility.AssertObjectIsActive(Findable.TITLECANVASNAME);
            PlayModeTestUtility.AssertObjectIsActive(Findable.SELECTPLAYMODECANVASNAME, isActive: false);
            PlayModeTestUtility.AssertObjectIsActive(Findable.SELECTBOARDCANVASNAME, isActive: false);
            PlayModeTestUtility.AssertObjectIsActive(Findable.CONFIGUREPLAYERSCANVASNAME, isActive: false);
            PlayModeTestUtility.AssertObjectIsActive(Findable.EDITPLAYERCANVASNMAE, isActive: false);
            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESCANVASNAME, isActive: false);
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
            yield return PlayModeTestUtility.LoadStartScene();
            yield return PlayModeTestUtility.LoadTitleScene();
            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESBUTTONNAME);

            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGEONENAME);
            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGETWONAME, isActive: false);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESFROMONETOTWOBUTTONNAME);

            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGETWONAME);
            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGEONENAME, isActive: false);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESFROMTWOTOONEBUTTONNAME);

            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGEONENAME);
            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGETWONAME, isActive: false);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESFROMONETOTWOBUTTONNAME);
            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESRETURNBUTTONNAME);

            PlayModeTestUtility.AssertObjectIsActive(Findable.TITLECANVASNAME);
            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESCANVASNAME, isActive: false);

            yield return PlayModeTestUtility.ClickOnButton(Findable.RULESBUTTONNAME);

            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGEONENAME);
            PlayModeTestUtility.AssertObjectIsActive(Findable.RULESPAGETWONAME, isActive: false);
        }

        [UnityTest]
        public IEnumerator PressStartOnTitle()
        {
            yield return PlayModeTestUtility.LoadStartScene();
            yield return PlayModeTestUtility.LoadTitleScene();
            yield return PlayModeTestUtility.ClickOnButton(Findable.STARTBUTTONNAME);
        }
    }
}
