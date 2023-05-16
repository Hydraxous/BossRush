using BepInEx;
using Discord;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BossRush
{
    [HarmonyPatch(typeof(DiscordController))]
    public static class DiscordRPCPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SendActivity")]
        public static bool Prefix(ref Activity ___cachedActivity)
        {
            if (!BossRushController.BossRushMode)
                return true;

            ___cachedActivity.Details = $"BOSS RUSH: {ParseDifficultyName(PrefsManager.Instance.GetInt("difficulty"))}";
            string deathString = (BossRushController.HardcoreMode) ? "HARDCORE" : $"DEATHS: {BossRushController.Deaths.ToString("000")}";
            ___cachedActivity.State = $"LAPS: {BossRushController.Laps.ToString("000")} | {deathString}";
            ___cachedActivity.Timestamps.Start = BossRushController.StartTimeEpoc;

            return true;
        }

        private static string ParseDifficultyName(int diff)
        {
            switch (diff)
            {
                case 0:
                    return "HARMLESS";
                case 1:
                    return "LENIENT";
                case 2:
                    return "STANDARD";
                case 3:
                    return "VIOLENT";
                case 4:
                    return "BRUTAL";
                case 5:
                    return "UKMD";

                default:
                    return "Game Journalist";
            }
        }
    }
}
