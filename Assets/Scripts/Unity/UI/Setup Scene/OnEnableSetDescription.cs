namespace Cascadia.Unity.UI
{
    using Cascadia.Unity.UI.SetupScene;
    using UnityEngine;

    public class OnEnableSetDescription : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private GameDescriber describer;

        private void OnEnable()
        {
            this.describer.UpdateDescriptionFromCurrentConfiguration();
        }
    }
}
