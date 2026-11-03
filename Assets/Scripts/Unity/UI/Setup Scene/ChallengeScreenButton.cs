namespace Cascadia.Unity.UI
{
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

        public ChallengeDefinition RepresentedDefinition { get; private set; }

        public void SetDefinition(ChallengeDefinition definition, ChallengeDefinitionDelegate onClick)
        {
            this.RepresentedDefinition = definition;
            this.nameLabel.text = definition.ChallengeName;
            this.button.onClick.AddListener(() => onClick(this.RepresentedDefinition));
        }
    }
}
