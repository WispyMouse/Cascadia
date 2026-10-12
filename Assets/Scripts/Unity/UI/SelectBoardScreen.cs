namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using Cascadia.Unity;
    using TMPro;

    public class SelectBoardScreen : MonoBehaviour
    {
        public Slider WidthSlider;
        public TMP_Text WidthValueLabel;

        public Slider HeightSlider;
        public TMP_Text HeightValueLabel;

        public Slider StallSlider;
        public TMP_Text StallValueLabel;

        private void Start()
        {
            this.WidthSlider.SetValueWithoutNotify(PersistentGameConfiguration.Singleton.Width);
            this.WidthValueLabel.text = PersistentGameConfiguration.Singleton.Width.ToString();
            this.HeightSlider.SetValueWithoutNotify(PersistentGameConfiguration.Singleton.Height);
            this.HeightValueLabel.text = PersistentGameConfiguration.Singleton.Height.ToString();
            this.StallSlider.SetValueWithoutNotify(PersistentGameConfiguration.Singleton.StallTurn);
            this.StallValueLabel.text = this.DescribeStallTurnLogic(PersistentGameConfiguration.Singleton.StallTurn);
        }

        public void UpdateFromSliders(int _)
        {
            PersistentGameConfiguration.Singleton.UpdateWidth(Mathf.RoundToInt(this.WidthSlider.value));
            this.WidthValueLabel.text = PersistentGameConfiguration.Singleton.Width.ToString();
            PersistentGameConfiguration.Singleton.UpdateHeight(Mathf.RoundToInt(this.HeightSlider.value));
            this.HeightValueLabel.text = PersistentGameConfiguration.Singleton.Height.ToString();
            PersistentGameConfiguration.Singleton.UpdateStall(Mathf.RoundToInt(this.StallSlider.value));
            this.StallValueLabel.text = this.DescribeStallTurnLogic(PersistentGameConfiguration.Singleton.StallTurn);
        }

        private string DescribeStallTurnLogic(int stallTurn)
        {
            if (stallTurn <= 0)
            {
                return $"[OFF]";
            }

            if (stallTurn == 1)
            {
                return $"1 turn";
            }

            return $"{stallTurn} turns";
        }
    }
}
