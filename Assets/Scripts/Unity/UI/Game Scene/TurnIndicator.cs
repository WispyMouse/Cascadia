namespace Cascadia.Unity.UI.GameScene
{
    using Cascadia.DataStructures;
    using TMPro;
    using UnityEngine;

    public class TurnIndicator : MonoBehaviour
    {
        [SerializeReference]
        private TurnOrderHolder turnOrderHolder;

        [SerializeReference]
        private TMP_Text turnOrderText;

        private void Awake()
        {
            this.turnOrderHolder.OnTurnStarted += this.OnPlayerTurn;
        }

        private void OnPlayerTurn(PlayerSide playerSide, int roundNumber)
        {
            this.turnOrderText.text = $"Round {roundNumber}\n" + PersistentGameConfiguration.Singleton.GetPlayerProfile(playerSide).PlayerName;
        }
    }
}
