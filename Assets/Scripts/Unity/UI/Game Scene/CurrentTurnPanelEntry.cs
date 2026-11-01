namespace Cascadia.Unity.UI.GameScene
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Profiling;
    using UnityEngine.UI;

    public class CurrentTurnPanelEntry : MonoBehaviour
    {
        public delegate void OnShowHintsDelegate(PlayerSide side);
        public delegate void OnHideHintsDelegate();

        public OnShowHintsDelegate OnShowHints;
        public OnHideHintsDelegate OnHideHints;

        public PlayerProfile RepresentedProfile { get; private set; }
        public PlayerSide RepresentedSide { get; private set; }

        public bool IsPlayerSideKnockedOut { get; private set; } = false;

        [Header("Internal Components")]
        [SerializeReference]
        private Image playerIcon;
        [SerializeReference]
        private GameObject isCurrentTurnIndicatorPanel;
        [SerializeReference]
        private GameObject isCurrentTurnIndicatorArrow;
        [SerializeReference]
        private GameObject knockoutPanel;
        [SerializeReference]
        private GameObject isNotTurnIndicator;
        [SerializeReference]
        private GameObject cascadeLevelTextHolder;
        [SerializeReference]
        private TMP_Text cascadeLevelText;
        [SerializeReference]
        private TMP_Text visibilityText;
        [SerializeField]
        private TMP_Text nameText;

        [Header("Exposed Properties")]
        public GameObject IsCurrentTurnIndicatorPanel => this.isCurrentTurnIndicatorPanel;
        public GameObject IsCurrnetTurnIndicatorArrow => this.IsCurrnetTurnIndicatorArrow;
        public Image PlayerIcon => this.playerIcon;

        [Header("Private Fields")]
        /// <summary>
        /// When determining how fast to make the blinking, subtract out the multiple this by the number of connections above one.
        /// This will make it blink faster.
        /// </summary>
        [SerializeField]
        private float cascadeLerpPerLevel = .2f;
        [SerializeField]
        private AnimationCurve cascadeLevelVisibilityCurve;
        [SerializeField]
        private float timeForCascadeLevelAtLevelOne = 1f;

        private float curCascadeLevelVisibility { get; set; } = 0;
        private float curCascadeTimeLevel { get; set; } = 0;


        public void RepresentProfile(PlayerProfile toRepresent, PlayerSide side)
        {
            this.Clear();

            this.RepresentedProfile = toRepresent;
            this.RepresentedSide = side;

            if (toRepresent.PlayerName == PersistentGameConfiguration.HUMAN)
            {
                this.nameText.gameObject.SetActive(false);
            }
            else
            {
                this.nameText.text = toRepresent.PlayerName;
                this.nameText.gameObject.SetActive(true);
            }

            // If we're still loading the icon, don't show it
            if (!this.RepresentedProfile.Sprite.NeedsResolution)
            {
                this.playerIcon.sprite = this.RepresentedProfile.Sprite.Sprite;
                this.playerIcon.gameObject.SetActive(true);
            }
            else
            {
                this.playerIcon.gameObject.SetActive(false);
                this.playerIcon.sprite = null;

                // Resolution not complete; wait for it and update it when it's finished
                this.RepresentedProfile.OnPlayerProfileSpriteUpdated += this.OnIconDoneLoading;
            }
        }

        public void Clear()
        {
            if (this.RepresentedProfile != null)
            {
                this.RepresentedProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;
            }

            this.RepresentedProfile = null;
            this.cascadeLevelTextHolder.SetActive(false);
            this.isCurrentTurnIndicatorPanel.SetActive(false);
            this.isCurrentTurnIndicatorArrow.SetActive(false);
            this.isNotTurnIndicator.SetActive(false);
            this.knockoutPanel.SetActive(false);
        }

        public void SetCascade(int level)
        {
            if (level == 0)
            {
                this.cascadeLevelTextHolder.SetActive(false);
                return;
            }

            this.cascadeLevelTextHolder.SetActive(true);
            this.cascadeLevelText.text = $"x{level}";

            // Blink faster for higher cascade levels
            this.curCascadeTimeLevel = this.timeForCascadeLevelAtLevelOne
                - Mathf.Lerp(0, this.timeForCascadeLevelAtLevelOne, this.cascadeLerpPerLevel * (level - 1));
        }

        public void MarkKnockout()
        {
            this.IsPlayerSideKnockedOut = true;
            this.knockoutPanel.SetActive(true);
            this.isNotTurnIndicator.SetActive(false);
            this.isCurrentTurnIndicatorArrow.SetActive(false);
            this.isCurrentTurnIndicatorPanel.SetActive(false);
        }

        // Marks the player as back in the game, visually.
        // HACK: Assumes that it would be this player's turn.
        public void RestorePlayer(PlayerSide sideToRestore)
        {
            this.IsPlayerSideKnockedOut = false;
            this.knockoutPanel.SetActive(false);
            this.isNotTurnIndicator.SetActive(sideToRestore.PlayerProfileIndex != this.RepresentedProfile.Index);

            bool isCurrentTurn = sideToRestore.PlayerProfileIndex == this.RepresentedProfile.Index;
            this.isCurrentTurnIndicatorArrow.SetActive(isCurrentTurn);
            this.isCurrentTurnIndicatorPanel.SetActive(isCurrentTurn);
        }

        public void SetCurrentTurn(bool isTurn)
        {
            // If we're knocked out, don't update the plates
            if (this.knockoutPanel.activeSelf)
            {
                return;
            }

            this.isCurrentTurnIndicatorPanel.SetActive(isTurn);
            this.isCurrentTurnIndicatorArrow.SetActive(isTurn);
            this.isNotTurnIndicator.SetActive(!isTurn);
        }

        private void Update()
        {
            if (!this.cascadeLevelTextHolder.activeSelf)
            {
                return;
            }

            this.curCascadeLevelVisibility = (this.curCascadeLevelVisibility + Time.deltaTime) % this.curCascadeTimeLevel;

            this.visibilityText.color = new Color(this.visibilityText.color.r, this.visibilityText.color.g, this.visibilityText.color.b,
                this.cascadeLevelVisibilityCurve.Evaluate(this.curCascadeLevelVisibility / this.curCascadeTimeLevel));
        }

        public void ShowHints() => this.OnShowHints?.Invoke(this.RepresentedSide);

        public void HideHints() => this.OnHideHints?.Invoke();

        private void OnIconDoneLoading()
        {
            this.RepresentedProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;

            this.playerIcon.sprite = this.RepresentedProfile.Sprite.Sprite;
            this.playerIcon.gameObject.SetActive(true);
        }
    }
}
