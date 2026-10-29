namespace Cascadia.Unity.UI.GameScene
{
    using System.Collections.Generic;
    using Cascadia.DataStructures;
    using UnityEngine;

    public class TurnOrderHolder : MonoBehaviour
    {
        public delegate void UITurnStarted(PlayerSide playerProfileIndex, int roundNumber);
        public UITurnStarted OnTurnStarted;

        public HashSet<int> SidesThatAreAI = new();

        [SerializeReference]
        private GameConductor gameConductor;
        public int PlayerCount => this.gameConductor.CurrentBoardState == null ? 0 : this.gameConductor.CurrentBoardState.BasedOnConfiguration.PlayerCount;

        public bool PlayerIsHuman(PlayerSide side) => !PersistentGameConfiguration.Singleton.GetPlayerProfile(side).IsAI;
    }
}
