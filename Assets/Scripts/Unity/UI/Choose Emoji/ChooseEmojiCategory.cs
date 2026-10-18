
namespace Cascadia.Unity.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    public class ChooseEmojiCategory : MonoBehaviour
    {
        [SerializeReference]
        private TMP_Text categoryLabel;

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
