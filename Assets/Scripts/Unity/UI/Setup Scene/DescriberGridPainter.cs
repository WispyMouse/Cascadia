namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;

    public class DescriberGridPainter : GridPainter<DescriberCellRenderer>
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Transform scalableTransform;

        [Header("Prefabs")]
        [SerializeReference]
        private DescriberCellRenderer cellPF;

        [Header("Private Fields")]
        [SerializeField]
        private float rootScale = 1f;
        [SerializeField]
        private float scalePerWidth = .5f;
        [SerializeField]
        private float distanceBetweenCells = 50f;

        [Header("Private Properties")]
        private List<DescriberCellRenderer> drawnCells { get; set; } = new();

        public override void Paint(GameConfiguration configuration)
        {
            this.scalableTransform.localScale = (this.rootScale + (configuration.Width * this.scalePerWidth)) * Vector3.one;
            base.Paint(configuration);
        }

        protected override DescriberCellRenderer GetCellPF()
        {
            return this.cellPF;
        }

        protected override void SetNewCellPosition(ICellRenderer toPosition, Coordinate coordinate, GameConfiguration forConfiguration)
        {
            float xOffset = -forConfiguration.Width / 2f * this.distanceBetweenCells;
            float yOffset = -forConfiguration.Height / 2f * this.distanceBetweenCells;
            Vector2 offset = new(xOffset, yOffset);
            Vector2 gain = new(coordinate.X * this.distanceBetweenCells, coordinate.Y * this.distanceBetweenCells);
            toPosition.SetLocalPosition(offset + gain, coordinate);
        }
    }
}
