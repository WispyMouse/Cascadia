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

        public void SetLocalPosition(Vector2 position, Coordinate coordinate)
        {
            this.transform.localPosition = position;
            this.Coordinate = coordinate;
        }
    }
}
