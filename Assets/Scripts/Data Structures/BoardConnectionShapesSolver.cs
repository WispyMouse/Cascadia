namespace Cascadia.DataStructures
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Theoretical collection of all possible "shapes" that can form connections.
    /// Used to cache possible solutions to TicTacToe boards.
    /// <see cref="BoardState"/> will keep this around to answer if incoming moves cause connections.
    /// </summary>
    public class BoardConnectionShapesSolver
    {
        /// <summary>
        /// Specification of a position and a radius.
        /// Used as a dictionary key for <see cref="coordinateToRadiusNearby"/>, such that we can quickly
        /// look up "nearby" coordinates quickly.
        /// </summary>
        private readonly struct GetCoordinatesAroundKey
        {
            /// <summary>
            /// Coordinate center position.
            /// </summary>
            public readonly Coordinate Position;

            /// <summary>
            /// Distance away from the <see cref="Position"/> to include.
            /// If zero, only include exactly <see cref="Position"/>.
            /// For every value above zero, expand the circle of coordinates included.
            /// </summary>
            public readonly int Radius;

            public GetCoordinatesAroundKey(Coordinate position, int radius)
            {
                this.Position = position;
                this.Radius = Math.Max(0, radius);
            }

            public override int GetHashCode() => HashCode.Combine(this.Position, this.Radius);
        }

        /// <summary>
        /// Lookup table of coordinates to area around it.
        /// Useful for quickly gathering neighbors.
        /// </summary>
        private readonly Dictionary<GetCoordinatesAroundKey, IReadOnlyCollection<Coordinate>> coordinateToRadiusNearby;
        public readonly IReadOnlyDictionary<Coordinate, IReadOnlyCollection<CellsConnection>> PossibleConnections;

        public BoardConnectionShapesSolver(GameConfiguration gameConfiguration)
        {
            this.coordinateToRadiusNearby = new Dictionary<GetCoordinatesAroundKey, IReadOnlyCollection<Coordinate>>(gameConfiguration.MaximumDimensions);
            Dictionary<Coordinate, HashSet<CellsConnection>> possibleConnections = new(gameConfiguration.MaximumDimensions);

            foreach (Coordinate coordinate in gameConfiguration.GetAllCoordinatesInMapDimensions())
            {
                possibleConnections.Add(coordinate, new HashSet<CellsConnection>());
            }

            // For every cell, check what directionality-shapes can possibly lead to a connection
            // If any part of this is out of bounds, that direction couldn't start from this shape
            // We want to put every direction every shape is involved in within lists, including when it doesn't start from the shape
            foreach (Coordinate rootedCoordinate in gameConfiguration.GetAllCoordinatesInMapDimensions())
            {
                foreach (DirectionalityVector directionality in BoardState.DirectionalitiesWithBackwards)
                {
                    // If this shape can't possibly extend to at least our in-a-row target, then this directionality
                    // can't be rooted from this cell
                    if (!this.DirectionalityIsInBounds(gameConfiguration, rootedCoordinate, directionality))
                    {
                        continue;
                    }

                    List<Coordinate> coordinatesInConnection = new(gameConfiguration.InARowToSolve);
                    List<CellsConnection> newConnections = new();

                    coordinatesInConnection.Add(rootedCoordinate);

                    for (int ii = 1; ii < gameConfiguration.InARowToSolve; ii++)
                    {
                        Coordinate resultingCoordinate = rootedCoordinate + (directionality * ii);

                        coordinatesInConnection.Add(resultingCoordinate);

                        // If we are at least long enough, mark the connection
                        if (ii < gameConfiguration.InARowToSolve - 1)
                        {
                            continue;
                        }

                        CellsConnection thisConnection = new(new List<Coordinate>(coordinatesInConnection), directionality);
                        newConnections.Add(thisConnection);
                    }

                    foreach (CellsConnection connection in newConnections)
                    {
                        foreach (Coordinate curCoordinate in connection.Cells)
                        {
                            possibleConnections[curCoordinate].Add(connection);
                        }
                    }
                }
            }

            Dictionary<Coordinate, IReadOnlyCollection<CellsConnection>> castConnections = new(possibleConnections.Count);

            foreach (Coordinate coordinate in possibleConnections.Keys)
            {
                castConnections.Add(coordinate, possibleConnections[coordinate]);
            }

            this.PossibleConnections = castConnections;
        }

        private bool DirectionalityIsInBounds(GameConfiguration gameConfiguration, Coordinate fromCell, DirectionalityVector fromDirection)
        {
            for (int ii = 1; ii < gameConfiguration.InARowToSolve; ii++)
            {
                Coordinate resultingCoordinate = fromCell + (fromDirection * ii);
                if (!gameConfiguration.SpotIsInBounds(resultingCoordinate))
                {
                    return false;
                }
            }

            return true;
        }

        public IReadOnlyCollection<Coordinate> GetCoordinatesAround(IReadOnlyBoardState currentGameState, Coordinate position, int radius)
        {
            GetCoordinatesAroundKey key = new(position, radius);

            if (this.coordinateToRadiusNearby.TryGetValue(key, out IReadOnlyCollection<Coordinate> coordinates))
            {
                return coordinates;
            }

            int span = (radius * 2) - 1;
            List<Coordinate> coordinatesToCheck = new(span);

            for (int xx = -radius; xx <= radius; xx++)
            {
                for (int yy = -radius; yy <= radius; yy++)
                {
                    Coordinate resultingCoordinate = new Coordinate(xx, yy) + position;

                    if (resultingCoordinate == position)
                    {
                        continue;
                    }

                    if (!currentGameState.SpotIsInBounds(resultingCoordinate))
                    {
                        continue;
                    }

                    coordinatesToCheck.Add(resultingCoordinate);
                }
            }

            this.coordinateToRadiusNearby.Add(key, coordinatesToCheck);

            return coordinatesToCheck;
        }
    }
}
