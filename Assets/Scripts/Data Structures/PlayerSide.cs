namespace Cascadia.DataStructures
{
    /// <summary>
    /// Represents a player in the game.
    /// This is a stripped down version that helps correleate to longer lasting player profiles.
    /// </summary>
    public class PlayerSide
    {
        /// <summary>
        /// The order in which this player acts.
        /// The first player is always "0", the second player is always "1", etc.
        /// </summary>
        public readonly int TurnOrderIndex;

        /// <summary>
        /// The index of the player profile for this player.
        /// When setting up the game, this is the index of the
        /// player profile that represents this side.
        /// </summary>
        public readonly int PlayerProfileIndex;

        public PlayerSide(int turnOrderIndex, int playerProfileIndex)
        {
            this.TurnOrderIndex = turnOrderIndex;
            this.PlayerProfileIndex = playerProfileIndex;
        }
    }
}
