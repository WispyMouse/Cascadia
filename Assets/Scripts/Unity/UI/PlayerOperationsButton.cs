namespace Cascadia.Unity.UI
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerOperationsButton : MonoBehaviour
    {
        public GameObject IconRepresenterBase;
        public Image IconOfPlayer;
        public TMP_Text TextOfPlayer;

        public GameObject ConfigureButtonBase;
        public GameObject PlusButtonBase;
        public GameObject MinusButtonBase;

        public PlayerProfile PlayerProfile { get; private set; }
        [SerializeReference]
        private PlayerConfiguratorSelector playerConfiguratorSelector;

        public void SetFromProfile(PlayerProfile profile)
        {
            this.PlayerProfile = profile;
            this.ConfigureButtonBase.SetActive(true);
            this.PlusButtonBase.SetActive(false);

            // Don't show minus for minimum players
            this.MinusButtonBase.SetActive(PersistentGameConfiguration.Singleton.Players.Count > BoardState.MINIMUMPLAYERS);

            // If we're still loading the icon, don't show it
            if (!profile.Sprite.NeedsResolution)
            {
                this.IconOfPlayer.sprite = profile.Sprite.Sprite;
                this.IconRepresenterBase.SetActive(true);
            }
            else
            {
                this.IconOfPlayer.gameObject.SetActive(false);
                this.IconRepresenterBase.SetActive(true);
                this.IconOfPlayer.sprite = null;

                // Resolution not complete; wait for it and update it when it's finished
                profile.OnPlayerProfileSpriteUpdated += this.OnIconDoneLoading;
            }

            this.TextOfPlayer.gameObject.SetActive(true);
            this.TextOfPlayer.text = profile.PlayerName;
        }

        public void OnMinus()
        {
            PersistentGameConfiguration.Singleton.RemovePlayer(this.PlayerProfile);
            this.PlayerProfile = null;
            this.ConfigureButtonBase.SetActive(false);
            this.PlusButtonBase.SetActive(true);
            this.MinusButtonBase.SetActive(false);
            this.IconRepresenterBase.SetActive(false);
            this.TextOfPlayer.gameObject.SetActive(false);

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
            this.ConfigureButtonBase.SetActive(false);
            this.PlusButtonBase.SetActive(showPlus);
            this.MinusButtonBase.SetActive(false);
            this.IconRepresenterBase.SetActive(false);
            this.TextOfPlayer.gameObject.SetActive(false);
        }

        private void OnIconDoneLoading()
        {
            this.PlayerProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;

            this.IconOfPlayer.sprite = this.PlayerProfile.Sprite.Sprite;
            this.IconOfPlayer.gameObject.SetActive(true);
            this.IconRepresenterBase.SetActive(true);
        }

        public bool IsShowingConfiguration => this.ConfigureButtonBase.activeInHierarchy;
    }
}
