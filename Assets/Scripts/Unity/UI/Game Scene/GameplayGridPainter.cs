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
        [SerializeField]
        private float distanceBetweenCells = 1;

        private Dictionary<Coordinate, GameCellRenderer> cells { get; set; } = new();

        public override void Paint(GameConfiguration configuration)
        {
            // Modify the in-world camera we're attached to to update orthographic sizing
            if (this.gridCamera != null)
            {
                float widthOrHeight = Mathf.Max(configuration.Width, configuration.Height);
                this.gridCamera.orthographicSize = (this.orthographicSizeBase + widthOrHeight) * this.orthographicSizeScalar;
            }

            // We want to "center" the GameplayGridPainter, both horizontal and vertical
            int maxWidth = configuration.Width;
            int maxHeight = configuration.Height;
            this.transform.localPosition = new(-maxWidth * this.distanceBetweenCells / 2f, -maxHeight * this.distanceBetweenCells / 2f);

            base.Paint(configuration);

            this.cells.Clear();
            this.cells.EnsureCapacity(this.InstantiatedCells.Count);

            foreach (GameCellRenderer curCell in this.InstantiatedCells)
            {
                this.cells.Add(curCell.Coordinate, curCell);
            }
        }

        protected override GameCellRenderer GetCellPF()
        {
            return this.cellPF;
        }

        protected override void SetNewCellPosition(GameCellRenderer toPosition, Coordinate coordinate, GameConfiguration forConfiguration)
        {
            toPosition.SetLocalPosition(new Vector2(coordinate.X * this.distanceBetweenCells, coordinate.Y * this.distanceBetweenCells), coordinate);
        }

        public IReadOnlyDictionary<Coordinate, GameCellRenderer> PositionToCells => this.cells;
    }
}
