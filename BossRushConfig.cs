using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Configuration;

namespace BossRush
{
    public static class BossRushConfig
    {
        public static ConfigEntry<float> LapBuffMultiplier { get; private set; }
        public static ConfigEntry<bool> EnemySpeedBuff { get; private set; } 
        public static ConfigEntry<bool> EnemyDamageBuff { get; private set; } 
        public static ConfigEntry<bool> EnemyHealthBuff { get; private set; } 

        public static ConfigEntry<bool> AlwaysShowStats { get; private set; }

        public static void Bind()
        {
            LapBuffMultiplier = BossRush.Instance.Config.Bind("EnemyBuff", "LapBuffMultiplier", 2.0f, "Applies X radiance tiers to enemies for every lap completed.");
            EnemySpeedBuff = BossRush.Instance.Config.Bind("EnemyBuff", "EnemySpeedBuff", false, "Buffs enemy speed on laps.");
            EnemyDamageBuff = BossRush.Instance.Config.Bind("EnemyBuff", "EnemyDamageBuff", true, "Buffs enemy damage on laps.");
            EnemyHealthBuff = BossRush.Instance.Config.Bind("EnemyBuff", "EnemyHealthBuff", true, "Buffs enemy health on laps.");
            AlwaysShowStats = BossRush.Instance.Config.Bind("General", "AlwaysShowStats", true, "Show always or only when pressing tab.");
        }
    }
}
