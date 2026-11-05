namespace Cascadia.Unity.UI
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Cascadia.Unity.UI.ChallengeScreen;

    public class ChallengeScreenButton : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private TMP_Text nameLabel;
        [SerializeReference]
        private Button button;
        [SerializeReference]
        private Transform clearIconParent;
        [SerializeReference]
        private Image clearIcon;

        [Header("Data Resources")]
        [SerializeField]
        private List<Sprite> clearSprites;

        public ChallengeDefinition RepresentedDefinition { get; private set; }

        public void SetDefinition(ChallengeDefinition definition, ChallengeDefinitionDelegate onClick)
        {
            this.RepresentedDefinition = definition;
            this.nameLabel.text = definition.ChallengeName;
            this.button.onClick.AddListener(() => onClick(this.RepresentedDefinition));

            this.clearIconParent.gameObject.SetActive(false);
        }

        public void SetClear(int clearCount)
        {
            this.clearIconParent.gameObject.SetActive(clearCount > 0);

            if (clearCount > 0)
            {
                int clampedIndex = Math.Min(clearCount - 1, this.clearSprites.Count);
                this.clearIcon.sprite = this.clearSprites[clampedIndex];
            }
        }

        public void Click()
        {
            this.button.onClick?.Invoke();
        }
    }
}
