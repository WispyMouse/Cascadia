namespace Cascadia.AIs
{
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Prefer Center Heuristic.asset", menuName = "COSTCO/AI Heuristic/Prefer Center")]
    public class PrefersCenterHeuristic : AIHeuristic
    {
        private int maxX;
        private int maxY;
        private Coordinate centerish;

        public override void BakeInformation(int forSide, IReadOnlyBoardState currentGameState)
        {
            // Integer division rounds, there might not be an exact center
            this.maxX = currentGameState.Width / 2;
            this.maxY = currentGameState.Height / 2;

            this.centerish = new Coordinate(this.maxX, this.maxY);
        }

        public override float ScorePosition(int forSide, IReadOnlyBoardState currentGameState, Coordinate position) => this.Weight * (2 - ((Mathf.Abs(this.centerish.Y - position.Y) / (float)this.maxX) + (Mathf.Abs(this.centerish.X - position.X) / (float)this.maxY)));
    }
}
