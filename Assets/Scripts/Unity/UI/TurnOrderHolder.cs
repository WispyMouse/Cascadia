namespace Cascadia.Unity.UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class TurnOrderHolder : MonoBehaviour
    {
        public delegate void UITurnStarted(int playerProfileIndex);
        public UITurnStarted OnTurnStarted;

        public HashSet<int> SidesThatAreAI = new();

        public GameConductor GameConductor;
        public int PlayerCount => this.GameConductor.CurrentBoardState == null ? 0 : this.GameConductor.CurrentBoardState.PlayerCount;

        public bool PlayerIsHuman(int index) => !PersistentGameConfiguration.Singleton.Players[index].IsAI;
    }
}
