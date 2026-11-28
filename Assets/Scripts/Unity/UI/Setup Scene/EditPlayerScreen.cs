namespace Cascadia.Unity.UI.SetupScene
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class EditPlayerScreen : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Image playerSpriteDisplay;
        [SerializeReference]
        private GameObject humanPanel;
        [SerializeReference]
        private TMP_InputField nameInputField;
        [SerializeReference]
        private GameObject aIPanel;
        [SerializeReference]
        private GameObject alreadyTakenPanel;
        [SerializeReference]
        private TMP_Text aICoreLabel;
        [SerializeReference]
        private ChooseEmojiCanvas chooseEmojiCanvas;

        [Header("Exposed Components")]
        public Sprite CurrentlyDisplayedPlayerSprite => this.playerSpriteDisplay.sprite;

        public PlayerProfile ShowingProfile { get; private set; }

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
            this.alreadyTakenPanel.SetActive(false);
            this.gameObject.SetActive(true);

            this.ShowingProfile = profile;

            if (this.ShowingProfile.PlayerName == PersistentGameConfiguration.HUMAN)
            {
                // Show the placeholder text
                this.nameInputField.SetTextWithoutNotify(string.Empty);
            }
            else
            {
                this.nameInputField.SetTextWithoutNotify(this.ShowingProfile.PlayerName);
            }

            if (this.ShowingProfile.IsAI)
            {
                this.aIPanel.SetActive(true);
                this.humanPanel.SetActive(false);
            }
            else
            {
                this.aIPanel.SetActive(false);
                this.humanPanel.SetActive(true);
            }

            this.UpdateAICore();
            profile.AssignSpriteWhenReady(this.playerSpriteDisplay);
        }

        public void ToggleHumanAI()
        {
            bool toggleAI = !this.ShowingProfile.IsAI;

            if (toggleAI)
            {
                this.ShowingProfile.SetAICore(0);
            }
            else
            {
                this.ShowingProfile.SetAICore(null);
            }

            this.OpenFromPlayer(this.ShowingProfile);
        }

        public void NameChanged(string newName) => this.ShowingProfile.SetName(newName);

        public void NextAICore()
        {
            this.ShowingProfile.SetAICore((this.ShowingProfile.AICoreIndex + 1) % PersistentGameConfiguration.Singleton.AICores.Count);
            this.UpdateAICore();
        }

        public void PreviousAICore()
        {
            this.ShowingProfile.SetAICore((this.ShowingProfile.AICoreIndex + PersistentGameConfiguration.Singleton.AICores.Count - 1) % PersistentGameConfiguration.Singleton.AICores.Count);
            this.UpdateAICore();
        }

        private void UpdateAICore()
        {
            if (this.ShowingProfile.IsAI)
            {
                this.aICoreLabel.gameObject.SetActive(true);
                this.aICoreLabel.text = this.ShowingProfile.AICore.name;
            }
            else
            {
                this.aICoreLabel.gameObject.SetActive(false);
            }
        }

        public async void OpenIconSelection()
        {
            await this.chooseEmojiCanvas.ShowForProfile(this.ShowingProfile);
        }
    }
}
