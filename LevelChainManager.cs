using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BossRush
{
    public static class LevelChainManager
    {
        private static LevelChainTable LevelChainTable { get; set; }
        public static Dictionary<string, string> LevelChainDictionary { get; private set; }

        //This allows for customized level chains, instead of the default one.
        public static void LoadLevelChainTable()
        {
            string filePath = BossRushPaths.BossRushLevelChain;
            if (!System.IO.File.Exists(filePath))
            {
                BossRush.BepInExLogger.LogInfo("No level chain file found. Writing default level chain.");
                WriteDefaultLevelChain();
                LevelChainTable = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelChainTable>(Properties.Resources.levelChainDefault);
            }
            else
            {
                string json = System.IO.File.ReadAllText(filePath);

                try
                {
                    LevelChainTable = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelChainTable>(json);
                    if(LevelChainTable == null)
                    {
                        throw new Exception("Invalid level chain file. Malformed Json?");
                    }

                    if(LevelChainTable.ModVersion != ConstInfo.VERSION)
                    {
                        throw new Exception("Level chain file version mismatch.");
                    }

                } catch (Exception e)
                {
                    //Use the default level chain if the file is corrupted.
                    BossRush.BepInExLogger.LogFatal("Error loading LevelChainTable. Details below.");
                    BossRush.BepInExLogger.LogFatal(e.Message + '\n' + e.StackTrace);
                    BossRush.BepInExLogger.LogError("Writing default level chain.");
                    WriteDefaultLevelChain();
                    LevelChainTable = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelChainTable>(Properties.Resources.levelChainDefault);
                }
            }

            LevelChainDictionary = LevelChainTable.ToDictionary();
        }

        private static void WriteDefaultLevelChain()
        {
            string filePath = BossRushPaths.BossRushLevelChain;
            string json = Properties.Resources.levelChainDefault;
            System.IO.File.WriteAllText(filePath, json);
        }

        public static string GetFirstLevelName()
        {
            return LevelChainTable.LevelChains[0].LevelFrom;
        }

        public static LevelChain GetChainOfLevel(string currentLevel)
        {
            return LevelChainTable.LevelChains.FirstOrDefault(x=>x.LevelFrom == currentLevel);
        }
    }
}
