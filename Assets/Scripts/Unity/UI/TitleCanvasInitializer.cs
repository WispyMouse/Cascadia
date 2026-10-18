namespace Cascadia.Unity.UI
{
    using System.Linq;
    using UnityEngine;

    public class TitleCanvasInitializer : MonoBehaviour
    {
        public string KeepOpen = Findable.TITLE.CANVASNAMES.TITLE;

        private void Awake()
        {
            bool anyOpened = false;
            Findable[] panels = GameObject.FindObjectsByType<Findable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (string panelName in Findable.TITLE.CANVASNAMES.TITLEMENUWINDOWS)
            {
                bool match = this.KeepOpen == panelName;
                anyOpened |= match;
                Findable panel = panels.FirstOrDefault(x => x.name == panelName);

                if (panel == null)
                {
                    Debug.LogError($"Missing panel {panelName}");
                    continue;
                }

                panel.gameObject.SetActive(match);
            }
        }
    }
}
