namespace Cascadia.Unity.UI
{
    using System.Collections;
    using Cascadia.DataStructures;
    using UnityEngine;

    public class AIThinkerLoop : MonoBehaviour
    {
        public GameConductor GameConductor;
        public TurnOrderHolder TurnOrderHolder;

        private Coroutine thinkingCoroutine { get; set; }

        public float TimeForAIToThinkBase = .1f;
        public AnimationCurve TimeForAIToThinkAdditionalSeconds;
        public AnimationCurve AdditionalTimeDuringCascade;

        private void Awake()
        {
            this.TurnOrderHolder.OnTurnStarted += this.OnTurnStarted;
        }

        private void OnTurnStarted(int turn)
        {
            if (this.thinkingCoroutine != null)
            {
                this.StopCoroutine(this.thinkingCoroutine);
                this.thinkingCoroutine = null;
            }

            if (!this.GameConductor.CurrentBoardState.AnyEmptySpots())
            {
                return;
            }

            if (this.GameConductor.CurrentBoardState == null || this.GameConductor.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End)
            {
                return;
            }

            // If the current player is human, do nothing
            if (this.TurnOrderHolder.PlayerIsHuman(turn))
            {
                return;
            }

            // If this side has been eliminated, skip them
            if (!this.GameConductor.CurrentBoardState.SidesStillInGame.Contains(turn))
            {
                return;
            }

            // If there are no possible moves, do nothing
            if (!this.GameConductor.CurrentBoardState.AnyEmptySpots())
            {
                return;
            }

            if (this.thinkingCoroutine != null)
            {
                Debug.Log($"Instructed to stop coroutine for thinking. This suggests we were told to make another AI turn action too early.");
                this.StopCoroutine(this.thinkingCoroutine);
            }

            this.thinkingCoroutine = this.StartCoroutine(this.AIThinksAndTakesTurn());
        }

        private IEnumerator AIThinksAndTakesTurn()
        {
            int side = this.GameConductor.CurrentBoardState.CurrentPlayerSide;
            float randomWait = this.TimeForAIToThinkAdditionalSeconds.Evaluate(Random.Range(0, 1f));

            if (this.GameConductor.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.Cascade)
            {
                randomWait += this.AdditionalTimeDuringCascade.Evaluate(Random.Range(0, 1f));
            }

            // Debug.Log($"Starting to think for {side}, waiting {randomWait} seconds...");

            yield return new WaitForSeconds(this.TimeForAIToThinkBase + randomWait);

            if (this.GameConductor.CurrentBoardState.CurrentGameState == BoardState.GameStateEnum.End)
            {
                yield break;
            }

            this.thinkingCoroutine = null;

            Coordinate move =
                PersistentGameConfiguration.Singleton.Players[side].AICore
                .DetermineMove(side, this.GameConductor.CurrentBoardState);

#if UNITY_EDITOR
            MoveCommand command = this.GameConductor.CurrentBoardState.GenerateCommandFromMove(side, move);
            Debug.Log($"({PersistentGameConfiguration.Singleton.Players[side].AICore.name}) Making move at {move}, expecting {command.ConnectionsMade.Count} connections");
#endif

            this.GameConductor.ChooseCell(move);
        }
    }
}
