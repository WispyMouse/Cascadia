namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.GameScene;
    using Cascadia.Unity.UI.Reusable;
    using UnityEngine;

    public class GameplayGridPainter : GridPainter<GameCellRenderer>
    {
        [Header("Scene Managers")]
        [SerializeReference]
        private Camera gridCamera;

        [Header("Prefabs")]
        [SerializeReference]
        private GameCellRenderer cellPF;

        [Header("Private Fields")]
        [SerializeField]
        private float orthographicSizeBase = 1f;
        [SerializeField]
        private float orthographicSizeScalar = .5f;
        private Dictionary<Coordinate, GameCellRenderer> cells { get; set; } = new();

        public override void Paint(GameConfiguration configuration)
        {
            // Modify the in-world camera we're attached to to update orthographic sizing
            if (this.gridCamera != null)
            {
                float widthOrHeight = Mathf.Max(configuration.Width, configuration.Height);
                this.gridCamera.orthographicSize = (this.orthographicSizeBase + widthOrHeight) * this.orthographicSizeScalar;
            }

            base.Paint(configuration);
        }

        protected override GameCellRenderer GetCellPF()
        {
            return this.cellPF;
        }

        protected override void SetNewCellPosition(ICellRenderer toPosition, Coordinate coordinate, GameConfiguration forConfiguration)
        {
            float xOffset = forConfiguration.Width / 2f;
            float yOffset = forConfiguration.Height / 2f;
            toPosition.SetLocalPosition(new Vector2(coordinate.X - xOffset, coordinate.Y - yOffset), coordinate);
        }

        public IReadOnlyDictionary<Coordinate, GameCellRenderer> PositionToCells => this.cells;
    }
}
