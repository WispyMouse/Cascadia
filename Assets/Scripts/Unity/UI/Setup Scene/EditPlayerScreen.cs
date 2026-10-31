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

            this.playerSpriteDisplay.sprite = this.ShowingProfile.Sprite.Sprite;

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

            this.UpdateIcon();
            this.UpdateAICore();
        }

        public void ToggleHumanAI()
        {
            bool toggleAI = !this.ShowingProfile.IsAI;

            if (toggleAI)
            {
                this.ShowingProfile.AICoreIndex = 0;
                this.ShowingProfile.PlayerName = this.ShowingProfile.AICore.name;
            }
            else
            {
                this.ShowingProfile.AICoreIndex = null;
                this.ShowingProfile.PlayerName = PersistentGameConfiguration.HUMAN;
            }

            this.OpenFromPlayer(this.ShowingProfile);
        }

        public void NameChanged(string newName) => this.ShowingProfile.PlayerName = newName;

        private void UpdateIcon()
        {
            this.playerSpriteDisplay.sprite = this.ShowingProfile.Sprite.Sprite;

            int countWithIndex = 0;

            // Check to see if there are any other players using the same icon; if they are, then we can't leave this screen yet!
            for (int ii = 0, count = PersistentGameConfiguration.Singleton.Players.Count; ii < count; ii++)
            {
                // Count everything else
                if (ii == this.ShowingProfile.Index)
                {
                    continue;
                }

                if (ReferenceEquals(PersistentGameConfiguration.Singleton.Players[ii].Sprite.Sprite, this.ShowingProfile.Sprite.Sprite))
                {
                    countWithIndex++;
                }
            }

            // If there was more than one with this icon, we can't allow you to select it
            this.alreadyTakenPanel.SetActive(countWithIndex > 1);
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
                this.aICoreLabel.gameObject.SetActive(true);
                this.ShowingProfile.PlayerName = this.ShowingProfile.AICore.name;
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
