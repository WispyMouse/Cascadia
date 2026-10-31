namespace Cascadia.Unity.UI.GameScene
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerOperationsButton : MonoBehaviour
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private PlayerConfiguratorSelector playerConfiguratorSelector;

        [Header("Internal Components")]
        [SerializeReference]
        private GameObject iconRepresenterBase;
        [SerializeReference]
        private Image iconOfPlayer;
        [SerializeReference]
        private TMP_Text textOfPlayer;

        [SerializeReference]
        private GameObject configureButtonBase;
        [SerializeReference]
        private Button configureButton;
        [SerializeReference]
        private GameObject plusButtonBase;
        [SerializeReference]
        private Button plusButton;
        [SerializeReference]
        private GameObject minusButtonBase;
        [SerializeReference]
        private Button minusButton;

        [Header("Exposed Components")]
        public Button PlusButton => this.plusButton;
        public Button MinusButton => this.minusButton;
        public Button ConfigureButton => this.configureButton;

        public PlayerProfile PlayerProfile { get; private set; }
        public bool IsShowingConfiguration => this.configureButtonBase.activeInHierarchy;

        public void SetFromProfile(PlayerProfile profile)
        {
            this.PlayerProfile = profile;
            this.configureButtonBase.SetActive(true);
            this.plusButtonBase.SetActive(false);

            // Don't show minus for minimum players
            this.minusButtonBase.SetActive(PersistentGameConfiguration.Singleton.Players.Count > BoardState.MINIMUMPLAYERS);

            // If we're still loading the icon, don't show it
            if (!profile.Sprite.NeedsResolution)
            {
                this.iconOfPlayer.sprite = profile.Sprite.Sprite;
                this.iconRepresenterBase.SetActive(true);
            }
            else
            {
                this.iconOfPlayer.gameObject.SetActive(false);
                this.iconRepresenterBase.SetActive(true);
                this.iconOfPlayer.sprite = null;

                // Resolution not complete; wait for it and update it when it's finished
                profile.OnPlayerProfileSpriteUpdated += this.OnIconDoneLoading;
            }

            this.textOfPlayer.gameObject.SetActive(true);
            this.textOfPlayer.text = profile.PlayerName;
        }

        public void OnMinus()
        {
            PersistentGameConfiguration.Singleton.RemovePlayer(this.PlayerProfile);
            this.PlayerProfile = null;
            this.configureButtonBase.SetActive(false);
            this.plusButtonBase.SetActive(true);
            this.minusButtonBase.SetActive(false);
            this.iconRepresenterBase.SetActive(false);
            this.textOfPlayer.gameObject.SetActive(false);

            this.playerConfiguratorSelector.UpdateFromMinusOrPlus();
        }

        public void OnPlus()
        {
            // If we're already subscribed, remove the callback for icon finishing
            if (this.PlayerProfile != null)
            {
                this.PlayerProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;
            }

            PlayerProfile newProfile = PersistentGameConfiguration.Singleton.AddPlayer();

            this.SetFromProfile(newProfile);
            this.playerConfiguratorSelector.UpdateFromMinusOrPlus();

            newProfile.OnPlayerProfileSpriteUpdated += this.OnIconDoneLoading;
        }

        public void Deactivate(bool showPlus)
        {
            if (this.PlayerProfile != null)
            {
                this.PlayerProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;
            }

            this.PlayerProfile = null;
            this.configureButtonBase.SetActive(false);
            this.plusButtonBase.SetActive(showPlus);
            this.minusButtonBase.SetActive(false);
            this.iconRepresenterBase.SetActive(false);
            this.textOfPlayer.gameObject.SetActive(false);
        }

        private void OnIconDoneLoading()
        {
            this.PlayerProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;

            this.iconOfPlayer.sprite = this.PlayerProfile.Sprite.Sprite;
            this.iconOfPlayer.gameObject.SetActive(true);
            this.iconRepresenterBase.SetActive(true);
        }

        public void ShowMinusButton(bool show = true)
        {
            this.minusButtonBase.SetActive(show);
        }
    }
}
