using System;
using System.Collections.Generic;
using System.Text;
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

        public static float TimeElapsed
        {
            get
            {
                float currentTime = Time.time;
                if(!BossRushMode)
                {
                    startTime = currentTime;
                }
                return currentTime - startTime;
            }
        }

        public static void StartBossRushMode(bool hardcore = false)
        {
            Reset();
            BossRushMode = true;
            HardcoreMode = hardcore;
            SceneHelper.LoadScene("Level 0-5");
        }

        public static void Reset()
        {
            BossRushMode = false;
            HardcoreMode = false;
            startTime = Time.time;
            Deaths = 0;
            Laps = 0;
        }
    }
}
