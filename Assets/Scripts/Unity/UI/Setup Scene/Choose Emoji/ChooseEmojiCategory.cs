namespace Cascadia.Unity.UI.SetupScene
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ChooseEmojiCategory : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private TMP_Text categoryLabel;
        [SerializeReference]
        private Button button;

        [Header("Exposed Components")]
        public Button Button => this.button;

        public string RepresentedCategory { get; private set; }

        public UnityEvent<string> OnCategoryClicked;

        public void SetCategory(string category)
        {
            this.RepresentedCategory = category;
            this.categoryLabel.text = category;
        }

        public void OnClick()
        {
            this.OnCategoryClicked?.Invoke(this.RepresentedCategory);
        }
    }
}
