namespace Cascadia.DataStructures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a "Connection", starting from the <see cref="Root"/> and ending at the <see cref="Tail"/>.
    /// </summary>
    public readonly struct CellsConnection : IEquatable<CellsConnection>
    {
        /// <summary>
        /// The <see cref="Coordinate"/> of cells making up this connection.
        /// Should be ordered such that <see cref="Root"/> is the 0th and <see cref="Tail"/> is the n-1th.
        /// </summary>
        public readonly IReadOnlyList<Coordinate> Cells;

        /// <summary>
        /// The starting point of this connection.
        /// </summary>
        public readonly Coordinate Root;

        /// <summary>
        /// The ending point of this connection.
        /// </summary>
        public readonly Coordinate Tail;

        /// <summary>
        /// What directional clear was used to determine this group?
        /// Can be used to join together same-Directionality Connections.
        /// </summary>
        public readonly DirectionalityVector Directionality;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cells">Cells contained in connection. Must contain at least one cell.</param>
        /// <param name="directionality">Directionality vector of the connection.</param>
        public CellsConnection(IReadOnlyList<Coordinate> cells, DirectionalityVector directionality)
        {
            int cellsCount = cells.Count;

            this.Cells = cells;
            this.Root = cells[0];
            this.Tail = cells[cellsCount - 1];

            this.Directionality = directionality;
        }

        public readonly bool Equals(CellsConnection other) => this.GetHashCode() == other.GetHashCode();

        public override readonly int GetHashCode() => HashCode.Combine(this.Root, this.Tail, this.Directionality);
    }
}
