namespace Cascadia.Unity.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.AIs;
    using Cascadia.Unity.UI.SetupScene;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ChallengeScreen : MonoBehaviour
    {
        public delegate void ChallengeDefinitionDelegate(ChallengeDefinition definition);

        [Header("Internal Components")]
        [SerializeReference]
        private GameObject playButton;
        [SerializeReference]
        private Transform challengeListParent;
        [SerializeReference]
        private GameDescriber challengeDescriber;

        [Header("Prefabs")]
        [SerializeReference]
        private ChallengeScreenButton challengeScreenButtonPF;

        [Header("Data Resources")]
        [SerializeField]
        private List<ChallengeDefinition> challenges;

        private bool firstTimeSetup { get; set; } = false;
        private ChallengeDefinition currentDefinition { get; set; }

        private void OnEnable()
        {
            if (!this.firstTimeSetup)
            {
                for (int ii = this.challengeListParent.childCount - 1; ii >= 0; ii--)
                {
                    Destroy(this.challengeListParent.GetChild(ii).gameObject);
                }

                for (int ii = 0, count = this.challenges.Count; ii < count; ii++)
                {
                    ChallengeScreenButton newButton = Instantiate(this.challengeScreenButtonPF, this.challengeListParent);
                    newButton.SetDefinition(this.challenges[ii], this.SelectDefinition);
                }
            }

            this.currentDefinition = null;
            this.challengeDescriber.Clear();
            this.playButton.SetActive(false);
        }

        private void SelectDefinition(ChallengeDefinition definition)
        {
            this.currentDefinition = definition;
            this.challengeDescriber.UpdateDescription(definition);
            this.playButton.SetActive(true);
        }

        public void PlayPressed()
        {
            PersistentGameConfiguration.Singleton.ForceSetConfig(this.currentDefinition.GetGameConfiguration());
            PersistentGameConfiguration.Singleton.EnsurePlayerCount(this.currentDefinition.AICores.Count + 1, true);

            PersistentGameConfiguration.Singleton.Players[0].AICoreIndex = null;

            for (int ii = 0; ii < this.currentDefinition.AICores.Count; ii++)
            {
                PersistentGameConfiguration.Singleton.Players[ii + 1].AICoreIndex = PersistentGameConfiguration.Singleton.AICores.IndexOf(this.currentDefinition.AICores[ii]);
                PersistentGameConfiguration.Singleton.Players[ii + 1].Sprite = new PlayerProfileSprite(this.currentDefinition.AISprites[ii], "?");
            }

            // HACK: Assumes play mode screen is the 1th entry
            SceneManager.LoadScene(1);
        }
    }
}
