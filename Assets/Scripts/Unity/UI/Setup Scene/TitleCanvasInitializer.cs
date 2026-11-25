namespace Cascadia.Unity.UI.SetupScene
{
    using System.Linq;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;

    public class TitleCanvasInitializer : MonoBehaviour
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private ChallengeScreen challengeScreen;
        [SerializeReference]
        private GameObject mainTitleScreen;

        private string panelToStartOpen { get; set; } = Findable.TITLE.CANVASNAMES.TITLE;

        private void Start()
        {
            if (PersistentGameConfiguration.Singleton.ChallengeIsSet)
            {
                this.panelToStartOpen = Findable.TITLE.CANVASNAMES.CHALLENGES;
            }

            bool anyOpened = false;
            Findable[] panels = GameObject.FindObjectsByType<Findable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (string panelName in Findable.TITLE.CANVASNAMES.TITLEMENUWINDOWS)
            {
                bool match = this.panelToStartOpen == panelName;
                anyOpened |= match;
                Findable panel = panels.FirstOrDefault(x => x.name == panelName);

                if (panel == null)
                {
                    Debug.LogError($"Missing panel {panelName}", this);
                    continue;
                }

                panel.gameObject.SetActive(match);
            }

            if (PersistentGameConfiguration.Singleton.ChallengeIsSet)
            {
                this.mainTitleScreen.SetActive(false);
                this.challengeScreen.gameObject.SetActive(true);
            }
        }
    }
}
