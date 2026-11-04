namespace Cascadia.DataStructures
{
    using System.Collections.Generic;
    using System.Linq;
    using Cascadia.Unity;

    [System.Serializable]
    public class SaveGame
    {
        public List<ChallengeClearData> ChallengeClearData = new();

        public int GetClearCount(string id)
        {
            int clearCount = 0;

            ChallengeClearData clearData = this.ChallengeClearData.FirstOrDefault(x => x.ChallengeId == id);

            if (clearData != null)
            {
                clearCount = clearData.ClearCountLevel;
            }

            return clearCount;
        }
    }
}
