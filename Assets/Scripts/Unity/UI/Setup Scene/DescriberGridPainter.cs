namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;

    public class DescriberGridPainter : GridPainter
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Transform scalableTransform;

        [Header("Private Fields")]
        [SerializeField]
        private float scalePerWidth = .5f;

        public override Dictionary<Coordinate, Cell> Paint(GameConfiguration configuration)
        {
            this.scalableTransform.localScale = Vector3.one / (configuration.Width * this.scalePerWidth);

            return base.Paint(configuration);
        }
    }
}
