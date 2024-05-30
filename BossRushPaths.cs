using System.IO;
using UnityEngine;

namespace BossRush
{
    public static class BossRushPaths 
    {
        public static string GetModLocation()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static string GameFolder => Path.GetDirectoryName(Application.dataPath);

        public static string BepInExFolder => Path.Combine(GameFolder, "BepInEx");
        public static string BepInExConfigFolder => Path.Combine(BepInExFolder, "config");
        public static string BossRushConfigFolder => Path.Combine(BepInExConfigFolder, "BossRush");
        public static string BossRushSaveFile => Path.Combine(BossRushConfigFolder, BossRushSaveFileName + ".json");
        public const string BossRushSaveFileName = "records";

        public static string BossRushLevelChain => Path.Combine(BossRushConfigFolder, "LevelChain.json");

        public static void CheckFolders()
        {
            CreateFolder(BossRushConfigFolder);
        }

        private static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
