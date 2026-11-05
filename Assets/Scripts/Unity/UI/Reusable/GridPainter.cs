namespace Cascadia.Unity.UI.Reusable
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using Cascadia.Unity.UI.GameScene;
    using UnityEngine;

    /// <summary>
    /// Draws a grid on command.
    /// </summary>
    public class GridPainter : MonoBehaviour
    {
        [Header("Internal Components")]
        [SerializeReference]
        private Transform gridParent;

        [Header("Prefabs")]
        [SerializeReference]
        private Cell cellPF;

        private List<Cell> instantiatedCells { get; set; } = new List<Cell>();

        public virtual Dictionary<Coordinate, Cell> Paint(GameConfiguration configuration)
        {
            Dictionary<Coordinate, Cell> cells = new(configuration.MaximumDimensions);

            for (int ii = this.instantiatedCells.Count - 1; ii >= 0; ii--)
            {
                Destroy(this.instantiatedCells[ii].gameObject);
            }

            this.instantiatedCells.Clear();

            float xOffset = configuration.Width / 2f;
            float yOffset = configuration.Height / 2f;

            for (int xx = 0; xx < configuration.Width; xx++)
            {
                for (int yy = 0; yy < configuration.Height; yy++)
                {
                    Coordinate position = new(xx, yy);
                    Cell newCell = Instantiate(this.cellPF, this.gridParent);
                    newCell.Position = position;
                    newCell.transform.localPosition = new Vector2(xx - xOffset, yy - yOffset);
                    this.instantiatedCells.Add(newCell);
                    cells.Add(position, newCell);
                }
            }

            return cells;
        }
    }
}
