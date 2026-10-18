namespace Cascadia.Unity.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ChooseEmojiIcon : MonoBehaviour
    {
        [SerializeReference]
        private Image emojiIcon;

        public Sprite RepresentedEmoji { get; private set; }

        public UnityEvent<ChooseEmojiIcon> OnEmojiClicked;

        [SerializeReference]
        private GameObject selectedIndicator;

        public void SetSprite(Sprite icon)
        {
            this.emojiIcon.sprite = icon;
            this.RepresentedEmoji = icon;
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
