namespace Cascadia.Unity.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Lerps an <see cref="Image"/>'s color between two values.
    /// Gives a glow-y effect.
    /// </summary>
    public class GlowingImage : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Image image;
        /// <summary>
        /// Given a % value for X, return a % value for the lerping value
        /// between <see cref="ColorA"/> and <see cref="ColorB"/>
        /// </summary>
        [SerializeField]
        private AnimationCurve colorTiming;

        [Header("Public Fields")]
        public Color ColorA;
        public Color ColorB;
        public float SecondsPerCycle = 1f;

        private float curSecondCycle { get; set; } = 0;

        private void Update()
        {
            this.curSecondCycle = (this.curSecondCycle + Time.deltaTime) % this.SecondsPerCycle;
            float curColorValue = this.colorTiming.Evaluate(this.curSecondCycle /  this.SecondsPerCycle);
            Color resultingColor = Color.Lerp(this.ColorA, this.ColorB, curColorValue);
            this.image.color = resultingColor;
        }
    }
}
