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

        private List<TCell> instantiatedCells { get; set; } = new List<TCell>();

        public virtual void Paint(GameConfiguration configuration)
        {
            Dictionary<Coordinate, TCell> cells = new(configuration.MaximumDimensions);

            for (int ii = this.instantiatedCells.Count - 1; ii >= 0; ii--)
            {
                Destroy(this.instantiatedCells[ii].SelfGameObject);
            }

            this.instantiatedCells.Clear();

            float xOffset = configuration.Width / 2f;
            float yOffset = configuration.Height / 2f;

            for (int xx = 0; xx < configuration.Width; xx++)
            {
                for (int yy = 0; yy < configuration.Height; yy++)
                {
                    Coordinate position = new(xx, yy);
                    TCell newCell = Instantiate(this.GetCellPF().SelfGameObject, this.gridParent).GetComponent<TCell>();
                    this.SetNewCellPosition(newCell, position, configuration);
                    this.instantiatedCells.Add(newCell);
                    cells.Add(position, newCell);
                }
            }
        }

        protected abstract void SetNewCellPosition(ICellRenderer toPosition, Coordinate coordinate, GameConfiguration forConfiguration);
        protected abstract TCell GetCellPF();
    }
}
