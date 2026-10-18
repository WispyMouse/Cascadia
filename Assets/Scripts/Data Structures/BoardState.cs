namespace Cascadia.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BoardState : IReadOnlyBoardState
    {
        public const int MINIMUMPLAYERS = 2;
        public const int MAXIMUMPLAYERS = 6;

        /// <summary>
        /// The directions that can source from a tile, and be considered as being "rooted" in that tile.
        /// When solving a piece's involvement, you could just iterate over each coordinate and look at
        /// connections going in these directions.
        /// </summary>
        public static IReadOnlyList<DirectionalityVector> Directionalities = new DirectionalityVector[]
        {
            new (1, 0),
            new (1, -1),
            new (0, -1),
            new (-1, -1),
        };

        /// <summary>
        /// <see cref="Directionalities"/>, but also pointing backwards.
        /// Helps in a practical way to find more connections.
        /// </summary>
        public static IReadOnlyList<DirectionalityVector> DirectionalitiesWithBackwards = new DirectionalityVector[]
        {
            new (1, 0),
            new (1, -1),
            new (0, -1),
            new (-1, -1),

            new (-1, 0),
            new (-1, 1),
            new (0, 1),
            new (1, 1),
        };

        public GameConfiguration BasedOnConfiguration => this.basedOnConfiguration;
        private readonly GameConfiguration basedOnConfiguration;

        public IReadOnlyList<MoveCommandConsequences> MoveCommandsApplied => this.moveCommandsApplied;
        private List<MoveCommandConsequences> moveCommandsApplied { get; set; } = new List<MoveCommandConsequences>();

        public IReadOnlyList<PlayerSide> PlayerOrder => this.playerOrder;
        private readonly List<PlayerSide> playerOrder = new();

        private readonly Dictionary<Coordinate, PlayerSide> spotToSideOwnership;

        int IReadOnlyBoardState.CurrentCascadeLevel => this.CurrentCascadeLevel;
        int IReadOnlyBoardState.CurrentPlayerIndex => this.CurrentPlayerTurnIndex;
        GameStateEnum IReadOnlyBoardState.CurrentGameState => this.CurrentGameState;
        BoardConnectionShapesSolver IReadOnlyBoardState.PossibilityContainer => this.PossibilityContainer;

        public int CurrentCascadeLevel { get; private set; }
        public int CurrentPlayerTurnIndex { get; private set; }
        public PlayerSide CurrentPlayerSide => this.PlayerOrder[this.CurrentPlayerTurnIndex];
        public HashSet<PlayerSide> PlayerSidesStillInGame { get; private set; } = new HashSet<PlayerSide>();
        public int CurrentRound { get; private set; } = 1;

        private readonly Dictionary<Coordinate, List<CellsConnection>> acceptedSolutions;

        public readonly BoardConnectionShapesSolver PossibilityContainer;
        public readonly BoardStateEvents BoardStateEvents;

        public enum GameStateEnum
        {
            /// <summary>
            /// This game has not started yet.
            /// </summary>
            NotStarted = 0,
            /// <summary>
            /// The game is progress.
            /// </summary>
            Playing = 1,
            /// <summary>
            /// Indicates the "cascade" state.
            /// Once in Cascade, players need to make at least <see cref="CurrentCascadeLevel"/>
            /// connections on their turn, or they are eliminated.
            /// </summary>
            Cascade = 2,
            /// <summary>
            /// Indicates that the game has ended.
            /// If <see cref="Winner"/> has no value, the game is a draw.
            /// Otherwise, that player index is declared the winner.
            /// </summary>
            End = 3
        }

        public GameStateEnum CurrentGameState;
        public PlayerSide Winner { get; set; }

        /// <summary>
        /// Constructor for a BoardState, the living definition of a game.
        /// </summary>
        /// <param name="gameConfiguration">The configuration to generate from. This cannot be changed during a game.</param>
        public BoardState(GameConfiguration gameConfiguration)
        {
            this.basedOnConfiguration = gameConfiguration;
            this.BoardStateEvents = new BoardStateEvents();

            this.spotToSideOwnership = new Dictionary<Coordinate, PlayerSide>(this.BasedOnConfiguration.MaximumDimensions);

            foreach (Coordinate coordinate in this.basedOnConfiguration.GetAllCoordinatesInMapDimensions())
            {
                this.spotToSideOwnership.Add(coordinate, null);
            }

            this.Winner = null;
            this.acceptedSolutions = new Dictionary<Coordinate, List<CellsConnection>>(this.BasedOnConfiguration.MaximumDimensions);

            this.CurrentGameState = GameStateEnum.NotStarted;
            this.CurrentCascadeLevel = 0;
            this.CurrentGameState = 0;

            if (gameConfiguration.ForceOrderlyTurn.HasValue)
            {
                this.AssignForceOrderlyTurn();
            }
            else
            {
                this.RandomlyAssignFirstPlayerTurn();
            }

            this.PossibilityContainer = new BoardConnectionShapesSolver(this.BasedOnConfiguration);
        }

        /// <summary>
        /// Clones a provided BoardState.
        /// Does not copy <see cref="BoardStateEvents"/>.
        /// </summary>
        public BoardState(BoardState toDeepClone) : this(toDeepClone.basedOnConfiguration)
        {
            this.BoardStateEvents = new BoardStateEvents();

            this.CurrentCascadeLevel = toDeepClone.CurrentCascadeLevel;
            this.CurrentPlayerTurnIndex = toDeepClone.CurrentPlayerTurnIndex;
            this.CurrentGameState = toDeepClone.CurrentGameState;
            this.PossibilityContainer = toDeepClone.PossibilityContainer;
            this.Winner = toDeepClone.Winner;

            this.spotToSideOwnership = new Dictionary<Coordinate, PlayerSide>(toDeepClone.spotToSideOwnership);
            this.PlayerSidesStillInGame = new HashSet<PlayerSide>(toDeepClone.PlayerSidesStillInGame);

            this.acceptedSolutions = new Dictionary<Coordinate, List<CellsConnection>>(this.acceptedSolutions.Count);
            foreach (Coordinate coordinate in toDeepClone.acceptedSolutions.Keys)
            {
                this.acceptedSolutions.Add(coordinate, new List<CellsConnection>(toDeepClone.acceptedSolutions[coordinate]));
            }
        }

        public IReadOnlyList<Coordinate> GetEmptySpots()
        {
            List<Coordinate> emptySpots = new(this.spotToSideOwnership.Count);

            foreach (KeyValuePair<Coordinate, PlayerSide> position in this.spotToSideOwnership)
            {
                if (position.Value == null)
                {
                    emptySpots.Add(position.Key);
                }
            }

            return emptySpots;
        }

        public bool AnyEmptySpots()
        {
            foreach (Coordinate position in this.spotToSideOwnership.Keys)
            {
                PlayerSide ownership = this.spotToSideOwnership[position];

                if (ownership == null)
                {
                    return true;
                }
            }

            return false;
        }

        public MoveCommandConsequences GenerateConsequencesFromMoveCommand(MoveCommand fromCommand)
        {
            IReadOnlyList<CellsConnection> connections = this.GetAllNewSolutions(fromCommand.Side, fromCommand.Position);
            List<PlayerSide> playerSidesRemoved = new();

            // HACK SHOULD GENERALIZE: Would this move result in a loss
            // "newConnections" are determined by the number of actual new connections
            // we want to only check new scoring connections in this process
            if (this.CurrentGameState == GameStateEnum.Cascade && this.CurrentCascadeLevel > connections.Count)
            {
                playerSidesRemoved.Add(fromCommand.Side);
            }

            return new MoveCommandConsequences(fromCommand, connections, this.CurrentCascadeLevel, playerSidesRemoved);
        }

        public IReadOnlyList<CellsConnection> GetAllNewSolutions(PlayerSide side, Coordinate hypotheticalPosition)
        {
            List<CellsConnection> newSolutions = new(DirectionalitiesWithBackwards.Count);

            IReadOnlyCollection<CellsConnection> possibleConnections = this.PossibilityContainer.PossibleConnections[hypotheticalPosition];

            foreach (CellsConnection possibleConnection in possibleConnections)
            {
                if (this.OverlayConnectionOwnedAfterPlacement(hypotheticalPosition, possibleConnection, side))
                {
                    newSolutions.Add(possibleConnection);
                }
            }

            return this.PruneSolutionsForNotAlreadySolved(newSolutions);
        }

        public bool SpotIsInBounds(Coordinate position) => this.BasedOnConfiguration.SpotIsInBounds(position);

        /// <summary>
        /// Attempts to apply a move command.
        /// If the move is not legal, an exception is thrown.
        /// Check legality using <see cref="MoveCommandLegalToPlay(MoveCommand)"/> first.
        /// </summary>
        public void ApplyMoveCommand(MoveCommand toApply, bool advancePlayer = true)
        {
            if (!this.MoveCommandLegalToPlay(toApply))
            {
                throw new ArgumentException($"Attempted to play illegal move. Check legality using {nameof(this.MoveCommandLegalToPlay)}.");
            }

            MoveCommandConsequences consequences = this.GenerateConsequencesFromMoveCommand(toApply);

            if (this.CurrentGameState == GameStateEnum.NotStarted)
            {
                this.CurrentGameState = GameStateEnum.Playing;
            }

            this.spotToSideOwnership[toApply.Position] = toApply.Side;
            this.moveCommandsApplied.Add(consequences);

            int solutionsCount = consequences.ConnectionsMade.Count;
            this.CurrentCascadeLevel = Math.Max(this.CurrentCascadeLevel, solutionsCount);

            // Notate all accepted connections
            foreach (CellsConnection connection in consequences.ConnectionsMade)
            {
                foreach (Coordinate coordinate in connection.Cells)
                {
                    if (!this.acceptedSolutions.TryGetValue(coordinate, out List<CellsConnection> existingConnections))
                    {
                        existingConnections = new List<CellsConnection>();
                        this.acceptedSolutions.Add(coordinate, existingConnections);
                    }
                    existingConnections.Add(connection);
                }
            }

            // Removed knocked out players, and consider end game state
            if (consequences.PlayerSidesRemoved.Any())
            {
                foreach (PlayerSide playerSideRemoved in consequences.PlayerSidesRemoved)
                {
                    // If there were no new solutions, or not enough solutions for previous cascade, and we're in cascade state,
                    // the current player should be knocked out
                    this.PlayerSidesStillInGame.Remove(playerSideRemoved);
                }

                // If there's only one remaining player, they must win
                if (this.PlayerSidesStillInGame.Count == 1)
                {
                    this.CurrentPlayerTurnIndex = this.PlayerSidesStillInGame.First().TurnOrderIndex;
                    this.Winner = this.CurrentPlayerSide;
                    this.CurrentGameState = GameStateEnum.End;

                    this.BoardStateEvents.OnGameConclusion?.Invoke(this.Winner);
                    return;
                }

                // If there are some how no remaining players, I guess this game is a draw
                if (this.PlayerSidesStillInGame.Count == 0)
                {
                    this.Winner = null;
                    this.CurrentGameState = GameStateEnum.End;

                    this.BoardStateEvents.OnGameConclusion?.Invoke(this.Winner);
                    return;
                }
            }
            else if (solutionsCount > 0)
            {
                this.CurrentGameState = GameStateEnum.Cascade;
            }

            if (!this.AnyLegalMovesRemainForPlayer(toApply.Side, out _))
            {
                this.CurrentGameState = GameStateEnum.End;
                this.Winner = null;

                // If there are no more empty spots, the winner is the player who most recently made a valid connection
                // We know they made a valid connection if the cascade level is above zero and they're still in the game
                // after their most recent move
                // If there haven't been any, the game is truly a draw
                if (this.CurrentCascadeLevel > 0)
                {
                    this.Winner = null;

                    for (int ii = 0; ii <= this.BasedOnConfiguration.PlayerCount; ii++)
                    {
                        // Count backwards through players to get most recent plays
                        int turnIndex = (this.CurrentPlayerTurnIndex - ii + this.BasedOnConfiguration.PlayerCount) % this.BasedOnConfiguration.PlayerCount;

                        if (!this.IsTurnOrderStillInGame(turnIndex))
                        {
                            continue;
                        }

                        this.Winner = this.playerOrder[turnIndex];
                    }

                    this.BoardStateEvents.OnGameConclusion?.Invoke(this.Winner);
                }
            }
            else
            {
                if (advancePlayer)
                {
                    this.AdvancePlayer();
                }
            }
        }

        private List<CellsConnection> PruneSolutionsForNotAlreadySolved(List<CellsConnection> solutions)
        {
            List<CellsConnection> remainingSolutions = new(solutions);

            // Check if any new solutions should be banded together; 4-in-a-row is the same value as a 3-in-a-row
            // Any solutions that have the same directionality *must* be bandable
            bool anyDiscarded;
            do
            {
                anyDiscarded = false;
                for (int leftSolutionIndex = remainingSolutions.Count - 2; leftSolutionIndex >= 0; leftSolutionIndex--)
                {
                    CellsConnection leftCellsSolution = remainingSolutions[leftSolutionIndex];
                    for (int rightSolutionIndex = remainingSolutions.Count - 1; rightSolutionIndex > leftSolutionIndex; rightSolutionIndex--)
                    {
                        CellsConnection rightCellsSolution = remainingSolutions[rightSolutionIndex];

                        // Forward or backward are the same
                        if (leftCellsSolution.Directionality == rightCellsSolution.Directionality || leftCellsSolution.Directionality == -rightCellsSolution.Directionality)
                        {
                            // Remove these two composite solutions, and note that they were removed
                            remainingSolutions.RemoveAt(rightSolutionIndex);
                            remainingSolutions.RemoveAt(leftSolutionIndex);

                            // Add a new composite solution to the end of the list, which won't be evaluated again
                            List<Coordinate> Sorted = SortByDirectionality(leftCellsSolution.Cells.Union(rightCellsSolution.Cells), leftCellsSolution.Directionality);
                            CellsConnection compositeSolution = new(Sorted, leftCellsSolution.Directionality);
                            remainingSolutions.Add(compositeSolution);
                            anyDiscarded = true;
                            break;
                        }
                    }

                    if (anyDiscarded)
                    {
                        break;
                    }
                }
            } while (anyDiscarded);

            do
            {
                anyDiscarded = false;

                // Next check if any of our new solutions contains pieces that already have that directionality solved
                for (int ii = remainingSolutions.Count - 1; ii >= 0; ii--)
                {
                    CellsConnection thisSolution = remainingSolutions[ii];

                    foreach (Coordinate curCoordinate in remainingSolutions[ii].Cells)
                    {
                        if (this.acceptedSolutions.TryGetValue(curCoordinate, out List<CellsConnection> existingConnections))
                        {
                            foreach (CellsConnection existingConnection in existingConnections)
                            {
                                if (existingConnection.Directionality == thisSolution.Directionality || existingConnection.Directionality == -thisSolution.Directionality)
                                {
                                    anyDiscarded = true;
                                    break;
                                }
                            }
                        }

                        if (anyDiscarded)
                        {
                            break;
                        }
                    }

                    if (anyDiscarded)
                    {
                        if (anyDiscarded)
                        {
                            remainingSolutions.RemoveAt(ii);
                            break;
                        }
                        break;
                    }
                }
            } while (anyDiscarded);

            return remainingSolutions;
        }

        public static List<Coordinate> SortByDirectionality(IEnumerable<Coordinate> coordinates, DirectionalityVector directionality)
        {
            // Place randomly in to list
            List<Coordinate> ordered = new(coordinates.Distinct());

            // Then sort by likeness to directionality
            ordered.Sort((x, y) => ((x.X * directionality.X) + (x.Y * directionality.Y)).CompareTo((y.X * directionality.X) + (y.Y * directionality.Y)));

            return ordered;
        }

        public bool TryGetConnectionsForCoordinate(Coordinate toGet, out IReadOnlyCollection<CellsConnection> connections)
        {
            if (this.acceptedSolutions.TryGetValue(toGet, out List<CellsConnection> writeableConnections))
            {
                connections = writeableConnections;
                return connections.Any();
            }

            connections = Array.Empty<CellsConnection>();
            return false;
        }

        public void ForceMarkOwnership(Coordinate position, PlayerSide ownership) => this.spotToSideOwnership[position] = ownership;

        public void RemoveConnection(Coordinate position, CellsConnection connection) => this.acceptedSolutions[position].Remove(connection);

        public bool OverlayConnectionOwned(CellsConnection connection, PlayerSide side)
        {
            bool allMatch = true;

            foreach (Coordinate curCell in connection.Cells)
            {
                if (this.GetCoordinateOwnership(curCell) != side)
                {
                    return false;
                }
            }

            return allMatch;
        }

        public bool OverlayConnectionOwnedAfterPlacement(Coordinate overrideCoordinate, CellsConnection connection, PlayerSide playerSide)
        {
            bool allMatch = true;

            foreach (Coordinate curCell in connection.Cells)
            {
                if (curCell == overrideCoordinate)
                {
                    continue;
                }

                if (this.GetCoordinateOwnership(curCell) != playerSide)
                {
                    return false;
                }
            }

            return allMatch;
        }

        public bool StallTurnEmbargoLifted()
        {
            if (this.BasedOnConfiguration.StallTurn <= 0)
            {
                return true;
            }

            return this.CurrentRound >= this.BasedOnConfiguration.StallTurn;
        }

        public bool MoveCommandLegalToPlay(MoveCommand toApply)
        {
            if (!this.SpotIsInBounds(toApply.Position))
            {
                return false;
            }

            if (!this.spotToSideOwnership.TryGetValue(toApply.Position, out PlayerSide ownership))
            {
                return false;
            }

            if (ownership != null)
            {
                return false;
            }

            // If the Stall Embargo is still in place, then a move that creates a connection is invalid
            if (!this.StallTurnEmbargoLifted())
            {
                MoveCommandConsequences consequences = this.GenerateConsequencesFromMoveCommand(toApply);
                if (consequences.ConnectionsMade.Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool AnyLegalMovesRemainForPlayer(PlayerSide playerSide, out IReadOnlyList<Coordinate> possiblePlays)
        {
            // No where left to play?
            if (!this.AnyEmptySpots())
            {
                possiblePlays = null;
                return false;
            }

            IReadOnlyList<Coordinate> emptyPlaces = this.GetEmptySpots();

            // Is there a "stall turn" setting? If not, then we're definitely okay to play
            if (this.StallTurnEmbargoLifted())
            {
                possiblePlays = emptyPlaces;
                return true;
            }

            // Check possible moves
            List<Coordinate> validPlaces = new(emptyPlaces.Count);

            foreach (Coordinate place in emptyPlaces)
            {
                MoveCommandConsequences command = this.GenerateConsequencesFromMoveCommand(new MoveCommand(playerSide, place));

                // If it makes any connections, it can't be played there
                if (command.ConnectionsMade.Count > 0)
                {
                    continue;
                }

                validPlaces.Add(place);
            }

            // Nowhere to play
            if (validPlaces.Count == 0)
            {
                possiblePlays = null;
                return false;
            }

            possiblePlays = validPlaces;
            return true;
        }

        public IReadOnlyBoardState DeepClone() => new BoardState(this);

        /// <summary>
        /// Reverse the previous move command, entirely undoing everything it did.
        /// The players in the game should be in the state they were before the move, as though it was never run.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="MoveCommand"/> that was undone from <see cref="MoveCommandsApplied"/>.
        /// Returns null if there was nothing that could be removed.
        /// </returns>
        public MoveCommandConsequences ReversePreviousMoveCommand()
        {
            int moveCommandsApplied = this.moveCommandsApplied.Count;
            if (moveCommandsApplied == 0)
            {
                return default;
            }

            MoveCommandConsequences toRemove = this.MoveCommandsApplied[moveCommandsApplied - 1];
            this.moveCommandsApplied.RemoveAt(moveCommandsApplied - 1);

            // HACK: At this point in the game's development, the only thing that *could* be placed during a reverse is a null
            // Will need to track what it used to be, if we can make it any other value while reversing
            this.ForceMarkOwnership(toRemove.FromCommand.Position, null);
            this.CurrentCascadeLevel = toRemove.PreviousCascadeLevel;

            // If anyone was knocked out, add them back to the game
            foreach (PlayerSide playerSideRemovedToRestore in toRemove.PlayerSidesRemoved)
            {
                this.PlayerSidesStillInGame.Add(playerSideRemovedToRestore);
                this.BoardStateEvents.OnPlayerRestored?.Invoke(playerSideRemovedToRestore);
            }

            // Remove any added connection
            foreach (CellsConnection connectionsAdded in toRemove.ConnectionsMade)
            {
                foreach (Coordinate coordinate in connectionsAdded.Cells)
                {
                    this.RemoveConnection(coordinate, connectionsAdded);
                }
            }

            if (this.MoveCommandsApplied.Count == 0)
            {
                this.CurrentGameState = GameStateEnum.NotStarted;
            }
            else if (this.CurrentCascadeLevel > 0)
            {
                this.CurrentGameState = GameStateEnum.Cascade;
            }
            else
            {
                this.CurrentGameState = GameStateEnum.Playing;
            }

            this.AdvancePlayer(reversePlayer: true);
            // We certainly no longer have a winner
            this.Winner = null;

            this.BoardStateEvents.OnMoveUndone?.Invoke(toRemove);

            return toRemove;
        }

        public void AdvancePlayer(bool reversePlayer = false)
        {
            if (!reversePlayer)
            {
                this.BoardStateEvents.OnPlayerMadeMove?.Invoke(this.CurrentPlayerSide);
            }

            // If we're advancing, add one. Reversing, minus one
            // This will modulo around the player count and let us find the next valid player
            int advancer = reversePlayer ? -1 : 1;

            for (int ii = 1; ii < this.BasedOnConfiguration.PlayerCount; ii++)
            {
                int previousPlayer = (this.CurrentPlayerTurnIndex + (advancer * (ii - 1)) + this.BasedOnConfiguration.PlayerCount) % this.BasedOnConfiguration.PlayerCount;
                int nextProspectivePlayer = (this.CurrentPlayerTurnIndex + (advancer * ii) + this.BasedOnConfiguration.PlayerCount) % this.BasedOnConfiguration.PlayerCount;

                // If the previous player is the starting player, and we're reversing, go back a round
                if (reversePlayer && previousPlayer == 0)
                {
                    this.CurrentRound--;
                }

                // If we're going forward, and it would become the starting player's turn, go forward a round
                if (!reversePlayer && nextProspectivePlayer == 0)
                {
                    this.CurrentRound++;
                }

                PlayerSide side = this.PlayerOrder[nextProspectivePlayer];

                if (!this.IsTurnOrderStillInGame(side.TurnOrderIndex))
                {
                    continue;
                }

                this.CurrentPlayerTurnIndex = nextProspectivePlayer;
                this.BoardStateEvents.OnPlayerStartTurn?.Invoke(this.CurrentPlayerSide);
                break;
            }
        }

        private void AssignForceOrderlyTurn()
        {
            // If there is a specific person who should go first, then everyone should be "in order"
            // and that player should be at the top of the list, index 0

            this.CurrentPlayerTurnIndex = this.BasedOnConfiguration.ForceOrderlyTurn.Value;
            List<PlayerSide> playerOrder = new(this.BasedOnConfiguration.PlayerCount);

            for (int ii = 0, count = this.BasedOnConfiguration.PlayerCount; ii < count; ii++)
            {
                int realIndex = (ii + this.CurrentPlayerTurnIndex + this.BasedOnConfiguration.PlayerCount) % this.BasedOnConfiguration.PlayerCount;

                PlayerSide newSide = new(ii, realIndex);
                this.playerOrder.Add(newSide);
                this.PlayerSidesStillInGame.Add(newSide);
            }
        }

        private void RandomlyAssignFirstPlayerTurn()
        {
            // Randomly select the player to move first
            // First, fill the list with all of the possible sides
            List<int> remainingSides = new(this.BasedOnConfiguration.PlayerCount);
            for (int ii = 0; ii < this.BasedOnConfiguration.PlayerCount; ii++)
            {
                remainingSides.Add(ii);
            }

            // Then dress down that list and randomly pull out of it
            int orderIndex = 0;
            while (remainingSides.Count > 0)
            {
                int randomIndex = new Random().Next(remainingSides.Count);
                int side = remainingSides[randomIndex];
                remainingSides.RemoveAt(randomIndex);
                PlayerSide newSide = new(orderIndex, side);
                this.playerOrder.Add(newSide);
                orderIndex++;
                this.PlayerSidesStillInGame.Add(newSide);
            }
        }

        public IEnumerable<PlayerSide> GetSidesStillInGame() => this.PlayerSidesStillInGame;

        public bool IsTurnOrderStillInGame(int sideIndex)
        {
            foreach (PlayerSide remainingSide in this.PlayerSidesStillInGame)
            {
                if (remainingSide.PlayerProfileIndex == sideIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public PlayerSide GetPlayerSideForTurnIndex(int turnIndex) => this.playerOrder[turnIndex];
        public PlayerSide GetPlayerSideForProfileIndex(int profileIndex)
        {
            foreach (PlayerSide side in this.playerOrder)
            {
                if (side.PlayerProfileIndex == profileIndex)
                {
                    return side;
                }
            }

            return null;
        }

        public PlayerSide GetCoordinateOwnership(Coordinate position)
        {
            return this.spotToSideOwnership[position];
        }
    }
}
