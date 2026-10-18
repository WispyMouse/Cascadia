namespace Cascadia.Unity.UI
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

        public Image PlayerIcon;

        public GameObject IsCurrentTurnIndicator;
        public GameObject KnockoutPanel;
        public GameObject IsNotTurnIndicator;

        public GameObject CascadeLevelTextHolder;
        public TMP_Text CascadeLevelText;

        public TMP_Text VisibilityText;
        public AnimationCurve CascadeLevelVisibilityCurve;
        public float TimeForCascadeLevelAtLevelOne = 1f;

        public TMP_Text NameText;
        private float curCascadeLevelVisibility { get; set; } = 0;
        private float curCascadeTimeLevel { get; set; } = 0;

        /// <summary>
        /// When determining how fast to make the blinking, subtract out the multiple this by the number of connections above one.
        /// This will make it blink faster.
        /// </summary>
        public float CascadeLerpPerLevel = .2f;

        public void RepresentProfile(PlayerProfile toRepresent, PlayerSide side)
        {
            this.Clear();

            this.RepresentedProfile = toRepresent;
            this.RepresentedSide = side;

            if (toRepresent.PlayerName == PersistentGameConfiguration.HUMAN)
            {
                this.NameText.gameObject.SetActive(false);
            }
            else
            {
                this.NameText.text = toRepresent.PlayerName;
                this.NameText.gameObject.SetActive(true);
            }

            // If we're still loading the icon, don't show it
            if (!this.RepresentedProfile.Sprite.NeedsResolution)
            {
                this.PlayerIcon.sprite = this.RepresentedProfile.Sprite.Sprite;
                this.PlayerIcon.gameObject.SetActive(true);
            }
            else
            {
                this.PlayerIcon.gameObject.SetActive(false);
                this.PlayerIcon.sprite = null;

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
            this.CascadeLevelTextHolder.SetActive(false);
            this.IsCurrentTurnIndicator.SetActive(false);
            this.IsNotTurnIndicator.SetActive(false);
            this.KnockoutPanel.SetActive(false);
        }

        public void SetCascade(int level)
        {
            if (level == 0)
            {
                this.CascadeLevelTextHolder.SetActive(false);
                return;
            }

            this.CascadeLevelTextHolder.SetActive(true);
            this.CascadeLevelText.text = $"x{level}";

            // Blink faster for higher cascade levels
            this.curCascadeTimeLevel = this.TimeForCascadeLevelAtLevelOne
                - Mathf.Lerp(0, this.TimeForCascadeLevelAtLevelOne, this.CascadeLerpPerLevel * (level - 1));
        }

        public void MarkKnockout()
        {
            this.KnockoutPanel.SetActive(true);
            this.IsNotTurnIndicator.SetActive(false);
            this.IsCurrentTurnIndicator.SetActive(false);
        }

        // Marks the player as back in the game, visually.
        // HACK: Assumes that it would be this player's turn.
        public void RestorePlayer(PlayerSide sideToRestore)
        {
            this.KnockoutPanel.SetActive(false);
            this.IsNotTurnIndicator.SetActive(sideToRestore.PlayerProfileIndex != this.RepresentedProfile.Index);
            this.IsCurrentTurnIndicator.SetActive(sideToRestore.PlayerProfileIndex == this.RepresentedProfile.Index);
        }

        public void SetCurrentTurn(bool isTurn)
        {
            // If we're knocked out, don't update the plates
            if (this.KnockoutPanel.activeSelf)
            {
                return;
            }

            this.IsCurrentTurnIndicator.SetActive(isTurn);
            this.IsNotTurnIndicator.SetActive(!isTurn);
        }

        private void Update()
        {
            if (!this.CascadeLevelTextHolder.activeSelf)
            {
                return;
            }

            this.curCascadeLevelVisibility = (this.curCascadeLevelVisibility + Time.deltaTime) % this.curCascadeTimeLevel;

            this.VisibilityText.color = new Color(this.VisibilityText.color.r, this.VisibilityText.color.g, this.VisibilityText.color.b,
                this.CascadeLevelVisibilityCurve.Evaluate(this.curCascadeLevelVisibility / this.curCascadeTimeLevel));
        }

        public void ShowHints() => this.OnShowHints?.Invoke(this.RepresentedSide);

        public void HideHints() => this.OnHideHints?.Invoke();

        private void OnIconDoneLoading()
        {
            this.RepresentedProfile.OnPlayerProfileSpriteUpdated -= this.OnIconDoneLoading;

            this.PlayerIcon.sprite = this.RepresentedProfile.Sprite.Sprite;
            this.PlayerIcon.gameObject.SetActive(true);
        }
    }
}
