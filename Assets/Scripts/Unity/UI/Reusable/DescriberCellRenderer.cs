namespace Cascadia.Unity.UI.Reusable
{
    using Cascadia.DataStructures;
    using UnityEngine;
    using UnityEngine.UI;

    public class DescriberCellRenderer : MonoBehaviour, ICellRenderer
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Image image;
        [SerializeReference]
        private RectTransform rectTransform;

        [Header("Private Fields")]
        [SerializeReference]
        private Sprite cellImageDefault;
        [SerializeReference]
        private Sprite cellImageHighlighted;

        public Coordinate Coordinate { get; private set; }

        public GameObject SelfGameObject => this.gameObject;

        public void SetHighlightStatus(bool toHighlight)
        {
            this.image.sprite = toHighlight ? this.cellImageHighlighted : this.cellImageDefault;
        }

        public void SetAnchorPosition(Coordinate coordinate, int maxSize)
        {
            float percentilePerPacing = 1f / maxSize;
            this.rectTransform.anchorMin = new Vector2(percentilePerPacing * coordinate.X, percentilePerPacing * coordinate.Y);
            this.rectTransform.anchorMax = new Vector2(percentilePerPacing * (coordinate.X + 1), percentilePerPacing * (coordinate.Y + 1));
            this.rectTransform.offsetMax = Vector2.zero;
            this.rectTransform.offsetMin = Vector2.zero;
            this.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
