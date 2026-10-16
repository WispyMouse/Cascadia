namespace Cascadia.DataStructures
{
    /// <summary>
    /// Represents all of the "events" that can happen in a BoardState.
    /// </summary>
    public class BoardStateEvents
    {
        public delegate void OnPlayerTurnDelegate(PlayerSide turn);
        public OnPlayerTurnDelegate OnPlayerStartTurn;
        public OnPlayerTurnDelegate OnPlayerMadeMove;

        public delegate void OnMoveUndoneDelegate(MoveCommandConsequences undone);
        public OnMoveUndoneDelegate OnMoveUndone;

        public delegate void OnGameConclusionDelegate(PlayerSide winner);
        public OnGameConclusionDelegate OnGameConclusion;
    }
}
