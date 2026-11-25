namespace Cascadia.Unity.UI
{
    using UnityEngine;

    public class ShowProperBackButtonOnEnable : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private GameObject mainMenuButton;
        [SerializeReference]
        private GameObject challengesButton;

        private void OnEnable()
        {
            if (PersistentGameConfiguration.Singleton.ChallengeIsSet)
            {
                this.mainMenuButton.SetActive(false);
                this.challengesButton.SetActive(true);
            }
            else
            {
                this.mainMenuButton.SetActive(true);
                this.challengesButton.SetActive(false);
            }
        }
    }
}
