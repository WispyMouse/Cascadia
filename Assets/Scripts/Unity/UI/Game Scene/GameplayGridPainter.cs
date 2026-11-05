namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.GameScene;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;

    public class GameplayGridPainter : GridPainter
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private Camera gridCamera;

        [Header("Private Fields")]
        [SerializeField]
        private float orthographicSizeBase = 1f;
        [SerializeField]
        private float orthographicSizeScalar = .5f;
        [SerializeField]
        private float widthOrthographicViewMultiplier = 1f;

        public override Dictionary<Coordinate, Cell> Paint(GameConfiguration configuration)
        {
            this.gridCamera.orthographicSize = this.orthographicSizeBase
                + (Mathf.Max(configuration.Width * this.widthOrthographicViewMultiplier, configuration.Height)
                * this.orthographicSizeScalar);

            return base.Paint(configuration);
        }
    }
}
