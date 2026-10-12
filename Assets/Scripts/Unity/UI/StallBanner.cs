using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cascadia.Unity.UI
{
    public class StallBanner : MonoBehaviour
    {
        public GameObject StallBannerGO;
        public Image Colorable;
        public TMP_Text StallBannerText;

        public Color NeutralColor;
        public Color AttentionColor;

        public float TimeForAttentionState = 3f;
        private float curTimeForAttentionState { get; set; } = 0;
        public AnimationCurve AttentionStateLerpColorTimer;

        public GameConductor GameConductor;
        public TurnOrderHolder TurnOrderHolder;

        private void Awake()
        {
            this.TurnOrderHolder.OnTurnStarted += this.OnTurnUpdate;
            this.Colorable.color = this.NeutralColor;
            this.Clear(null);
        }

        public void OnTurnUpdate(int _)
        {
            this.Colorable.color = this.NeutralColor;

            if (this.GameConductor.CurrentBoardState.StallTurnEmbargoLifted())
            {
                this.StallBannerGO.SetActive(false);
                return;
            }

            this.StallBannerGO.SetActive(true);

            int remainingTurns = this.GameConductor.CurrentBoardState.StallTurn - this.GameConductor.CurrentBoardState.CurrentRound;

            if (remainingTurns == 1)
            {
                this.StallBannerText.text = $"No Connections For: 1 More Turn";
            }
            else
            {
                this.StallBannerText.text = $"No Connections For: {remainingTurns} More Turns";
            }
        }

        public void Clear(int? _) => this.StallBannerGO.SetActive(false);

        public void CallAttention() => this.curTimeForAttentionState = this.TimeForAttentionState;

        private void Update()
        {
            if (this.curTimeForAttentionState <= 0)
            {
                this.Colorable.color = this.NeutralColor;
                return;
            }

            this.curTimeForAttentionState -= Time.deltaTime;
            this.Colorable.color = Color.Lerp(this.NeutralColor, this.AttentionColor, this.curTimeForAttentionState / this.TimeForAttentionState);
        }
    }
}
