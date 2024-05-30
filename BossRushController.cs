using System;
using UnityEngine;

namespace BossRush
{
    public static class BossRushController
    {
        public static bool BossRushMode { get; internal set; }
        public static bool HardcoreMode { get; internal set; }
        public static int Laps { get; internal set; }
        public static int Deaths { get; internal set; }

        private static float startTime;

        public static long StartTimeEpoc { get; internal set; }

        //Returns time since the Boss Rush started.
        public static float TimeElapsed
        {
            get
            {
                float currentTime = Time.time;
                if (!BossRushMode)
                    startTime = currentTime;

                return currentTime - startTime;
            }
        }

        //Starts Boss Rush mode.
        public static void StartBossRushMode(bool hardcore = false)
        {
            Reset();
            BossRushMode = true;
            HardcoreMode = hardcore;
            SceneHelper.LoadScene(LevelChainManager.GetFirstLevelName());
        }

        //Resets stats and info
        public static void Reset()
        {
            BossRushMode = false;
            HardcoreMode = false;
            StatRecords.HighScoreRun = false;
            startTime = Time.time;
            StartTimeEpoc = DateTimeOffset.Now.ToUnixTimeSeconds();
            Deaths = 0;
            Laps = 0;
        }

        public static BossRushRecord GetCurrentStat()
        {
            return new BossRushRecord(Laps, TimeElapsed, Deaths, HardcoreMode);
        }
    }
}
