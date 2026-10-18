namespace Cascadia.Unity.UI
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class EditPlayerScreen : MonoBehaviour
    {
        public PlayerProfile ShowingProfile { get; private set; }

        public Image PlayerSpriteDisplay;

        public GameObject HumanPanel;
        public TMP_InputField NameInputField;

        public GameObject AIPanel;

        public GameObject AlreadyTakenPanel;

        public TMP_Text AICoreLabel;

        [SerializeReference]
        private ChooseEmojiCanvas chooseEmojiCanvas;

        public void OpenFromPlayerProfileIndex(int profileIndex)
        {
            PlayerProfile profile = PersistentGameConfiguration.Singleton.Players[profileIndex];
            this.OpenFromPlayer(profile);
        }

        private void OnEnable()
        {
            // When re-enabling, update the player. This is useful if we've changed their icon in the inner menu.
            if (this.ShowingProfile != null)
            {
                this.OpenFromPlayer(this.ShowingProfile);
            }
        }

        public void OpenFromPlayer(PlayerProfile profile)
        {
            this.AlreadyTakenPanel.SetActive(false);
            this.gameObject.SetActive(true);

            this.ShowingProfile = profile;

            if (this.ShowingProfile.PlayerName == PersistentGameConfiguration.HUMAN)
            {
                // Show the placeholder text
                this.NameInputField.SetTextWithoutNotify(string.Empty);
            }
            else
            {
                this.NameInputField.SetTextWithoutNotify(this.ShowingProfile.PlayerName);
            }

            this.PlayerSpriteDisplay.sprite = this.ShowingProfile.Sprite.Sprite;

            if (this.ShowingProfile.IsAI)
            {
                this.AIPanel.SetActive(true);
                this.HumanPanel.SetActive(false);
            }
            else
            {
                this.AIPanel.SetActive(false);
                this.HumanPanel.SetActive(true);
            }

            this.UpdateIcon();
            this.UpdateAICore();
        }

        public void ToggleHumanAI()
        {
            this.ShowingProfile.IsAI = !this.ShowingProfile.IsAI;

            if (this.ShowingProfile.IsAI)
            {
                this.ShowingProfile.AICoreIndex = 0;
                this.ShowingProfile.PlayerName = this.ShowingProfile.AICore.name;
            }
            else
            {
                this.ShowingProfile.PlayerName = PersistentGameConfiguration.HUMAN;
            }

            this.OpenFromPlayer(this.ShowingProfile);
        }

        public void NameChanged(string newName) => this.ShowingProfile.PlayerName = newName;

        private void UpdateIcon()
        {
            this.PlayerSpriteDisplay.sprite = this.ShowingProfile.Sprite.Sprite;

            int countWithIndex = 0;

            for (int ii = 0, count = PersistentGameConfiguration.Singleton.Players.Count; ii < count; ii++)
            {
                if (PersistentGameConfiguration.Singleton.Players[ii].Sprite.Sprite == this.ShowingProfile.Sprite.Sprite)
                {
                    countWithIndex++;
                }
            }

            // If there was more than one with this icon, we can't allow you to select it
            this.AlreadyTakenPanel.SetActive(countWithIndex > 1);
        }

        public void NextAICore()
        {
            this.ShowingProfile.AICoreIndex = (this.ShowingProfile.AICoreIndex + 1) % PersistentGameConfiguration.Singleton.AICores.Count;
            this.UpdateAICore();
        }

        public void PreviousAICore()
        {
            this.ShowingProfile.AICoreIndex = (this.ShowingProfile.AICoreIndex + PersistentGameConfiguration.Singleton.AICores.Count - 1) % PersistentGameConfiguration.Singleton.AICores.Count;
            this.UpdateAICore();
        }

        private void UpdateAICore()
        {
            if (this.ShowingProfile.IsAI)
            {
                this.AICoreLabel.gameObject.SetActive(true);
                this.ShowingProfile.PlayerName = this.ShowingProfile.AICore.name;
                this.AICoreLabel.text = this.ShowingProfile.AICore.name;
            }
            else
            {
                this.AICoreLabel.gameObject.SetActive(false);
            }
        }

        public async void OpenIconSelection()
        {
            await this.chooseEmojiCanvas.ShowForProfile(this.ShowingProfile);
        }
    }
}
