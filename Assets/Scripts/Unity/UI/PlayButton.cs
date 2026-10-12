namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    public class PlayButton : MonoBehaviour
    {
        public void OnPress() =>
            // HACK: Assumes play mode screen is the 1th entry
            SceneManager.LoadScene(1);
    }
}
