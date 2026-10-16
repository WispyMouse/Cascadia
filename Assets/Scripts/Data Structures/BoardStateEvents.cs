namespace Cascadia.DataStructures
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents all of the "events" that can happen in a BoardState.
    /// </summary>
    public class BoardStateEvents
    {
        public delegate void PlayerSideDelegate(PlayerSide turn);
        public PlayerSideDelegate OnPlayerStartTurn;
        public PlayerSideDelegate OnPlayerMadeMove;

        /// <summary>
        /// The player was eliminated, but for some reason is back in the game.
        /// Probably because of an undo action.
        /// </summary>
        public PlayerSideDelegate OnPlayerRestored;

        public delegate void OnMoveUndoneDelegate(MoveCommandConsequences undone);
        public OnMoveUndoneDelegate OnMoveUndone;

        public delegate void OnGameConclusionDelegate(PlayerSide winner);
        public OnGameConclusionDelegate OnGameConclusion;
    }
}
