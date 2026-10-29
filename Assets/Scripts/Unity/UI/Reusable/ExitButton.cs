namespace Cascadia.Unity.UI.Reusable
{
    using UnityEngine;

    /// <summary>
    /// Closes the game.
    /// </summary>
    public class ExitButton : MonoBehaviour
    {
        public void OnPress()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
