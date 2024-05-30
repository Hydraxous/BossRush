using System;
using System.Collections.Generic;
using System.Linq;

namespace BossRush
{
    [Serializable]
    public class LevelChainTable
    {
        public string ModVersion;
        public List<LevelChain> LevelChains;
        public Dictionary<string, string> ToDictionary()
        {
            return LevelChains.ToDictionary(x => x.LevelFrom, x => x.LevelTo);
        }
    }

    [Serializable]
    public class LevelChain
    {
        public string LevelFrom;
        public string LevelTo;
        //Needed for preventing player from going to the next level without defeating the boss via a secret exit.
        public string PitTargetFilter;
    }
}
