namespace Cascadia.Unity
{
    using System.Collections.Generic;
    using Cascadia.AIs;
    using Cascadia.DataStructures;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Challenge.asset", menuName = "Cascadia/Challenge Definition")]
    public class ChallengeDefinition : ScriptableObject
    {
        public string Identifier;
        public string ChallengeName;
        public List<AICore> AICores = new();
        public List<Sprite> AISprites = new();

        public int Width;
        public int Height;
        public int StallTurn;
        public int ForceOrderlyTurnRaw;
        public int? ForceOrderlyTurn => this.ForceOrderlyTurnRaw < 0 ? null : this.ForceOrderlyTurnRaw;

        public virtual GameConfiguration GetGameConfiguration()
        {
            return new GameConfiguration(this.Width, this.Height, this.AICores.Count + 1, 3, this.StallTurn, this.ForceOrderlyTurn);
        }

        public List<PlayerProfile> GetPlayerProfiles()
        {
            List<PlayerProfile> profiles = new(this.AICores.Count + 1);

            // Add the human profile
            // Should always be the first entry
            PersistentGameConfiguration.Singleton.EnsurePlayerCount(this.AICores.Count + 1, true);
            profiles.Add(PersistentGameConfiguration.Singleton.Players[0]);

            for (int ii = 0; ii < this.AICores.Count; ii++)
            {
                PlayerProfile newProfile = PersistentGameConfiguration.Singleton.Players[ii + 1];
                newProfile.SetAICore(PersistentGameConfiguration.Singleton.AICores.IndexOf(this.AICores[ii]), this.AISprites[ii]);
                profiles.Add(newProfile);
            }

            return profiles;
        }
    }
}
