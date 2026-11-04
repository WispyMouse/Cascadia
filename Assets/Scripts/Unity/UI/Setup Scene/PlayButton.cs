namespace Cascadia.Unity.UI.SetupScene
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    public class PlayButton : MonoBehaviour
    {
        public void OnPress()
        {
            // If we're playing through this button, we must not be doing a challenge
            PersistentGameConfiguration.Singleton.ClearChallenge();

            // HACK: Assumes play mode screen is the 1th entry
            SceneManager.LoadScene(1);
        }
    }
}
