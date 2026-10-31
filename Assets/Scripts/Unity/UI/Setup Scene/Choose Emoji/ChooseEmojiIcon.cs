namespace Cascadia.Unity.UI.SetupScene
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ChooseEmojiIcon : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Image emojiIcon;
        [SerializeReference]
        private GameObject selectedIndicator;
        [SerializeReference]
        private Button button;

        [Header("Exposed Components")]
        public Button Button => this.button;

        public Sprite RepresentedEmoji { get; private set; }

        public UnityEvent<ChooseEmojiIcon> OnEmojiClicked;

        public void SetSprite(Sprite icon)
        {
            this.emojiIcon.sprite = icon;
            this.RepresentedEmoji = icon;

            this.gameObject.name = $"{nameof(ChooseEmojiIcon)}: {icon.name}";
        }

        public void OnClick()
        {
            this.OnEmojiClicked?.Invoke(this);
        }

        public void SetSelectionStatus(bool status)
        {
            this.selectedIndicator.SetActive(status);
        }
    }
}
