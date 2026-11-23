namespace Cascadia.Unity.UI.GameScene
{
    using System.Threading;
    using System.Threading.Tasks;
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

        [Header("Private Properties")]
        private SemaphoreSlim getPlayerSemaphore { get; set; } = new SemaphoreSlim(1);

        public PlayerProfile PlayerProfile { get; private set; }
        public bool IsShowingConfiguration => this.configureButtonBase.activeInHierarchy;

        public void SetFromProfile(PlayerProfile profile)
        {
            this.PlayerProfile = profile;
            this.configureButtonBase.SetActive(true);
            this.plusButtonBase.SetActive(false);

            // Don't show minus for minimum players
            this.minusButtonBase.SetActive(PersistentGameConfiguration.Singleton.Players.Count > BoardState.MINIMUMPLAYERS);

            this.textOfPlayer.gameObject.SetActive(true);
            this.textOfPlayer.text = profile.PlayerName;

            if (profile.HasSprite)
            {
                this.iconOfPlayer.sprite = profile.Sprite.Value.Sprite;
                this.iconRepresenterBase.SetActive(true);
            }
            else
            {
                void SetSpriteAfterLoad(PlayerProfile loaded)
                {
                    if (this.iconOfPlayer == null)
                    {
                        return;
                    }

                    this.iconOfPlayer.sprite = loaded.Sprite.Value.Sprite;
                    this.iconRepresenterBase.SetActive(true);
                }

                profile.OnPlayerProfileSpriteLoaded += SetSpriteAfterLoad;
            }
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

        public async void OnPlus()
        {
            await this.OnPlusAsync();
        }

        public async Task OnPlusAsync()
        {
            await this.getPlayerSemaphore.WaitAsync();

            PlayerProfile newProfile = PersistentGameConfiguration.Singleton.AddPlayer();
            this.SetFromProfile(newProfile);
            this.playerConfiguratorSelector.UpdateFromMinusOrPlus();

            while (!newProfile.HasSprite)
            {
                await Task.Delay(1);
            }

            this.getPlayerSemaphore.Release();
        }

        public void Deactivate(bool showPlus)
        {
            this.PlayerProfile = null;
            this.configureButtonBase.SetActive(false);
            this.plusButtonBase.SetActive(showPlus);
            this.minusButtonBase.SetActive(false);
            this.iconRepresenterBase.SetActive(false);
            this.textOfPlayer.gameObject.SetActive(false);
        }

        public void ShowMinusButton(bool show = true)
        {
            this.minusButtonBase.SetActive(show);
        }
    }
}
