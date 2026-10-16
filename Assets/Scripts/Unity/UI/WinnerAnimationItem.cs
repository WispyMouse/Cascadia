namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.UI;

    public class WinnerAnimationItem : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve sizePossibilities;
        [SerializeField]
        private AnimationCurve speedPossibliities;

        [SerializeReference]
        private Image image;
        public Image Image => this.image;

        private float chosenSize { get; set; }
        private float chosenSpeed { get; set; }

        private void Awake()
        {
            this.chosenSize = this.sizePossibilities.Evaluate(Random.Range(0, 1f));
            this.chosenSpeed = this.speedPossibliities.Evaluate(Random.Range(0, 1f)) * (Random.Range(0, 1f) >= .5f ? 1f : -1f);

            this.transform.localScale = Vector2.one * this.chosenSize;
        }

        private void Update()
        {
            this.transform.localRotation = this.transform.localRotation * Quaternion.Euler(0f, 0f, this.chosenSpeed * Time.deltaTime);
        }
    }
}
