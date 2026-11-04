namespace Cascadia.Unity.UI
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ChallengeBanner : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Transform bannerParent;
        [SerializeReference]
        private Image award;
        [SerializeReference]
        private TMP_Text challengeTitle;

        [Header("Data Resources")]
        [SerializeField]
        private List<Sprite> clearSprites = new();

        private async void Start()
        {
            if (PersistentGameConfiguration.Singleton.ChallengeIsSet)
            {
                this.bannerParent.gameObject.SetActive(false);
                return;
            }

            this.bannerParent.gameObject.SetActive(true);
            this.challengeTitle.text = PersistentGameConfiguration.Singleton.CurrentChallenge.ChallengeName;

            int clearCount = await SaveGameManager.Singleton.GetClearCount(PersistentGameConfiguration.Singleton.CurrentChallenge);
            this.UpdateIconForClearCount(clearCount);
        }

        public void UpdateIconForClearCount(int clearCount)
        {
            this.award.gameObject.SetActive(clearCount > 0);

            if (clearCount > 0)
            {
                int clampedIndex = Math.Min(clearCount - 1, this.clearSprites.Count);
                this.award.sprite = this.clearSprites[clampedIndex];
            }
        }
    }
}
