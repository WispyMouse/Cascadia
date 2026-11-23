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

        public override void Paint(GameConfiguration configuration)
        {
            base.Paint(configuration);
        }

        protected override DescriberCellRenderer GetCellPF()
        {
            return this.cellPF;
        }

        protected override void SetNewCellPosition(DescriberCellRenderer toPosition, Coordinate coordinate, GameConfiguration forConfiguration)
        {
            // Grab the largest dimension
            int maxDimensionSize = Mathf.Max(forConfiguration.Height, forConfiguration.Width);
            toPosition.SetAnchorPosition(coordinate, maxDimensionSize);
        }
    }
}
