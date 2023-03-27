using BossRush.UI;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BossRush
{
    [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.StartLoadingNextLevel))]
    public static class FinalRankPatch
    {
        private static Dictionary<string, string> levelAssociations = new Dictionary<string, string>()
        {
            { "Level 1-1", "Level 1-4"}, //Cerb -> V2
            { "Level 2-1", "Level 2-4"}, //V2 -> Minos Corpse
            { "Level 3-1", "Level 3-2" }, //Minos Corpse -> Gabriel
            { "Intermission1", "Level 4-4" }, //Gabriel -> V2
            { "Level 5-1", "Level 5-4" }, //V2 -> Leviathan
            { "Level 6-1", "Level 6-2"}, //Leviathan -> Gabriel 2
            { "Intermission2", "Level P-1"}, //Gabriel 2 -> Minos Prime
            { "Level 3-2", "Level P-2" }, //Minos Prime -> Sisyphus Prime
            { "Level 6-2", "Level 0-5" }, //Sisyphus Prime -> Cerb LOOP
        };

        public static bool Prefix(FinalRank __instance)
        {
            if (BossRushController.BossRushMode && !SceneHelper.IsPlayingCustom)
                __instance.targetLevelName = ResolveLevelName(__instance.targetLevelName);

            return true;
        }

        public static string ResolveLevelName(string targetLevel)
        {
            string nextLevel = targetLevel;

            if(levelAssociations.ContainsKey(targetLevel))
            {
                nextLevel = levelAssociations[targetLevel];
                if(nextLevel == "Level 0-5")
                {
                    ++BossRushController.Laps;
                }
            }else
            {
                HudMessageReceiver.Instance.SendHudMessage("Sequence broken, boss rush mode disabled.");
                BossRushController.Reset();
            }
            return nextLevel;
        }
    }

    [HarmonyPatch(typeof(OptionsManager), nameof(OptionsManager.QuitMission))]
    public static class QuitMissionPatch
    { 
        public static void Postfix()
        {
            BossRushController.Reset();
        }
    }

    [HarmonyPatch(typeof(NewMovement))]
    public static class HardcoreResetPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(NewMovement.GetHurt))]
        public static bool Prefix(NewMovement __instance, int damage, bool invincible)
        {
            if (!BossRushController.HardcoreMode || __instance.gameObject.layer != 15 || damage <= 0 || !BossRushController.BossRushMode)
            {
                return true;
            }

            //Restart if hardcore and died :p
            if(__instance.hp - damage <= 0)
            {
                BossRushController.StartBossRushMode(true);
                return false;
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(NewMovement.Respawn))]
        public static void Postfix(NewMovement __instance)
        {
            if(!BossRushController.BossRushMode)
                return;

            ++BossRushController.Deaths;
        }
    }

    [HarmonyPatch(typeof(EnemyIdentifier), "Start")]
    public static class EnemyDifficultyUpPatch
    {
        public static void Postfix(EnemyIdentifier __instance)
        {
            if(BossRushController.Laps <= 0 || !BossRushController.BossRushMode)
            {
                return;
            }

            __instance.healthBuff = BossRushConfig.EnemyHealthBuff.Value;
            __instance.speedBuff = BossRushConfig.EnemySpeedBuff.Value;
            __instance.damageBuff = BossRushConfig.EnemyDamageBuff.Value;

            __instance.radianceTier = (BossRushController.Laps * BossRushConfig.LapBuffMultiplier.Value);
        }
    }

    //Set the ui object to be watched.
    [HarmonyPatch(typeof(LevelStatsEnabler), "Start")]
    public static class StatsPatch
    {
        public static void Postfix(LevelStatsEnabler __instance)
        {
            BossRushStats.LevelStatsUI = __instance.transform.GetChild(0).gameObject;
        }
    }

    [HarmonyPatch(typeof(CanvasController), "Awake")]
    public static class BossRushButtonPatch
    {
        public static void Postfix(CanvasController __instance)
        {

            if (SceneManager.GetActiveScene().name != "Main Menu")
                return;

            SpawnMenu(__instance);
            SpawnButton(__instance);           
        }
        
        private static void SpawnMenu(CanvasController __instance)
        {
            RectTransform rt = __instance.GetComponent<RectTransform>();
            if (rt == null)
                return;

            GameObject.Instantiate(Assets.BossRushMenuPrefab, rt);
        }

        private static void SpawnButton(CanvasController __instance)
        {
            RectTransform rt = __instance.GetComponent<RectTransform>();
            GameObject chapterSelect = rt.Find("Chapter Select").gameObject;
            if (chapterSelect == null)
            {
                Debug.Log("chap null");
                return;
            }

            BossRushMenu.SetLastPage(chapterSelect);
            RectTransform chap_rt = chapterSelect.GetComponent<RectTransform>();
            GameObject sandboxButton = chapterSelect.transform.Find("Sandbox").gameObject;

            if (sandboxButton == null)
            {
                Debug.Log("chap null");
                return;
            }

            GameObject bossRushButtonGO = GameObject.Instantiate(sandboxButton, chap_rt);
            bossRushButtonGO.name = "Boss Rush Button";
            Button bossRushButton = bossRushButtonGO.GetComponent<Button>();

            ColorBlock colors = bossRushButton.colors;
            GameObject.DestroyImmediate(bossRushButton);

            Button newButton = bossRushButtonGO.AddComponent<Button>();
            newButton.colors = colors;


            RectTransform bossRush_rt = bossRushButtonGO.GetComponent<RectTransform>();

            Vector3 currentPos = bossRush_rt.position;
            currentPos.y -= 55;
            bossRush_rt.position = currentPos;

            bossRush_rt.GetComponentInChildren<Text>().text = "BOSS RUSH";

            newButton.onClick.AddListener(BossRushMenu.Open);
        }
    }

}
