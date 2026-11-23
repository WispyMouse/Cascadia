namespace Cascadia.Unity.UI.Reusable
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.GameScene;
    using UnityEngine;

    /// <summary>
    /// Draws a grid on command.
    /// </summary>
    public abstract class GridPainter<TCell> : MonoBehaviour where TCell : ICellRenderer
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Transform gridParent;

        protected List<TCell> InstantiatedCells { get; set; } = new List<TCell>();

        public virtual void Paint(GameConfiguration configuration)
        {
            Dictionary<Coordinate, TCell> cells = new(configuration.MaximumDimensions);

            this.Clear();

            float xOffset = configuration.Width / 2f;
            float yOffset = configuration.Height / 2f;

            for (int xx = 0; xx < configuration.Width; xx++)
            {
                for (int yy = 0; yy < configuration.Height; yy++)
                {
                    Coordinate position = new(xx, yy);
                    TCell newCell = Instantiate(this.GetCellPF().SelfGameObject, this.gridParent).GetComponent<TCell>();
                    this.SetNewCellPosition(newCell, position, configuration);
                    this.InstantiatedCells.Add(newCell);
                    cells.Add(position, newCell);
                }
            }
        }

        protected abstract void SetNewCellPosition(TCell toPosition, Coordinate coordinate, GameConfiguration forConfiguration);
        protected abstract TCell GetCellPF();

        public void Clear()
        {
            for (int ii = this.InstantiatedCells.Count - 1; ii >= 0; ii--)
            {
                Destroy(this.InstantiatedCells[ii].SelfGameObject);
            }
            this.InstantiatedCells.Clear();
        }
    }
}
