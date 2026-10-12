namespace Cascadia.Unity.UI
{
    using UnityEngine;

    /// <summary>
    /// Easy access component for triggering a simple UI blip.
    /// </summary>
    public class BlipPlayerInvoker : MonoBehaviour
    {
        public void PlayBlip() => RandomBlipPlayer.Singleton.PlayRandomSound();
    }
}
