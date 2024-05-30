using BossRush.UI;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BossRush
{
    [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.LevelChange))]
    public static class FinalRankPatch
    {
        //Hijack the target level from FinalRank when exiting a level and swap it with the one we want.
        public static bool Prefix(FinalRank __instance)
        {
            if (BossRushController.BossRushMode && !SceneHelper.IsPlayingCustom)
            {
                LevelChain chain = LevelChainManager.GetChainOfLevel(SceneHelper.CurrentScene);
                
                if(chain == null)
                {
                    //Level has no chain!? What?
                    BossRush.BepInExLogger.LogWarning($"Level {SceneHelper.CurrentScene} has no valid chain.");
                    return true;
                }

                if (!string.IsNullOrEmpty(chain.PitTargetFilter) && __instance.targetLevelName != chain.PitTargetFilter)
                {
                    //This can happen if the player enters a secret level pit
                    HudMessageReceiver.Instance.SendHudMessage("Sequence broken, boss rush mode disabled.");
                    BossRushController.Reset();
                }
                else
                {
                    string nextLevel = chain.LevelTo;
                    __instance.targetLevelName = nextLevel;

                    //Increment laps if we're at the end of the chain.
                    if (nextLevel == LevelChainManager.GetFirstLevelName())
                    {
                        ++BossRushController.Laps;
                        StatRecords.SubmitRecord(BossRushController.GetCurrentStat());
                    }
                }
            }

            return true;
        }
    }

    //Disables boss rush if mission is quit, if hardcore is enabled, restarting mission will send the player back to the start.
    [HarmonyPatch(typeof(OptionsManager))]
    public static class OptionsManagerFixes
    {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(OptionsManager.QuitMission))]
        public static void Postfix()
        {
            BossRushController.Reset();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(OptionsManager.RestartMission))]
        public static bool Prefix()
        {
            if (!BossRushController.BossRushMode)
                return true;


            if(BossRushController.HardcoreMode)
            {
                BossRushController.StartBossRushMode(true);
                return false;
            }

            BossRushController.Deaths++;

            return true;
        }
    }

    [HarmonyPatch(typeof(StatsManager))]
    public static class StasManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(StatsManager.Restart))]
        public static void Postfix()
        {
            if (!BossRushController.BossRushMode)
                return;

            if (BossRushController.HardcoreMode)
            {
                BossRushDeathScreen.Instance.Open();
                return;
            }

            BossRushController.Deaths++;
        }
    }

    [HarmonyPatch(typeof(NewMovement))]
    public static class HardcoreResetPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(NewMovement.GetHurt))]
        public static bool Prefix(NewMovement __instance, int damage, bool invincible)
        {
            if (!BossRushController.HardcoreMode || !BossRushController.BossRushMode)
                return true;

            if (__instance.gameObject.layer == 15)
                return true;

            //Restart if hardcore and dead
            if (__instance.hp - damage <= 0)
            {
                try
                {
                    BossRushDeathScreen.Instance.Open();
                } catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return true;
                }
                return false;
            }

            return true;
        }
    }


    //For laps, difficulty is increased via radiance.
    [HarmonyPatch(typeof(EnemyIdentifier), "Start")]
    public static class EnemyDifficultyUpPatch
    {
        public static void Postfix(EnemyIdentifier __instance)
        {
            if (BossRushController.Laps <= 0 || !BossRushController.BossRushMode)
                return;

            __instance.healthBuff = BossRushConfig.EnemyHealthBuff.Value;
            __instance.speedBuff = BossRushConfig.EnemySpeedBuff.Value;
            __instance.damageBuff = BossRushConfig.EnemyDamageBuff.Value;

            __instance.radianceTier = (BossRushController.Laps * BossRushConfig.LapBuffMultiplier.Value);
        }
    }

    //Set the ui object to be watched for the boss rush stats UI
    //LevelStatsUI is watched if it is active and can potentially control if our BossRush stats are displayed based on the mod config
    [HarmonyPatch(typeof(LevelStatsEnabler), "Start")]
    public static class StatsPatch
    {
        public static void Postfix(LevelStatsEnabler __instance)
        {
            BossRushStats.UK_LevelStatsObject = __instance.transform.GetChild(0).gameObject;
        }
    }

    //Adds our modded menu and button to the game's Main Menu
    [HarmonyPatch(typeof(CanvasController), "Awake")]
    public static class MainMenuUIPatch
    {
        public static void Postfix(CanvasController __instance)
        {
            SpawnDeathScreen(__instance);

            if (SceneHelper.CurrentScene == "Main Menu")
            {
                SpawnMenu(__instance);
                SpawnButton(__instance);
            }
        }

        //Instantiates the Boss Rush Menu
        private static void SpawnMenu(CanvasController __instance)
        {
            RectTransform canvasRectTransform = __instance.GetComponent<RectTransform>();

            if (canvasRectTransform == null)
                return;

            BossRush.BepInExLogger.LogInfo("Menu Spawned");
            GameObject.Instantiate(Assets.BossRushMenuPrefab, canvasRectTransform);
        }

        private static void SpawnDeathScreen(CanvasController __instance)
        {
            RectTransform canvasRectTransform = __instance.GetComponent<RectTransform>();

            if (canvasRectTransform == null)
                return;

            BossRush.BepInExLogger.LogInfo("DeathScreen Spawned");
            GameObject.Instantiate(Assets.BossRushDeathScreen, canvasRectTransform);
        }


        //Duplicates sandbox button and configures it to open Boss rush menu
        //Kind of icky but it works.
        private static void SpawnButton(CanvasController __instance)
        {
            BossRush.BepInExLogger.LogInfo("Spawning Button");
            RectTransform canvasRectTransform = __instance.GetComponent<RectTransform>();
            GameObject chapterSelectObject = canvasRectTransform.Find("Chapter Select").gameObject;
            if (chapterSelectObject == null)
            {
                Debug.LogError("Chapter Select object is null");
                return;
            }

            BossRushMenu.SetLastPage(chapterSelectObject);
            RectTransform chapterSelectRectTransform = chapterSelectObject.GetComponent<RectTransform>();
            GameObject sandboxButtonObject = chapterSelectObject.transform.Find("Sandbox").gameObject;

            if (sandboxButtonObject == null)
            {
                Debug.LogError("Sandbox button is null");
                return;
            }

            GameObject bossRushButtonObject = GameObject.Instantiate(sandboxButtonObject, chapterSelectRectTransform);
            bossRushButtonObject.name = "Boss Rush Button";
            Button sandboxButton = bossRushButtonObject.GetComponent<Button>();

            ColorBlock oldColorBlock = sandboxButton.colors;
            //Have to destroy old button component because of Unity's persistent listener calls.
            //They can't be removed at runtime so the component must be replaced.
            GameObject.DestroyImmediate(sandboxButton);

            Button bossRushButton = bossRushButtonObject.AddComponent<Button>();
            bossRushButton.colors = oldColorBlock;


            RectTransform bossRushButtonRectTransform = bossRushButtonObject.GetComponent<RectTransform>();

            Vector3 buttonPosition = bossRushButtonRectTransform.position;
            buttonPosition.y -= 55;
            bossRushButtonRectTransform.position = buttonPosition;

            bossRushButtonRectTransform.GetComponentInChildren<TextMeshProUGUI>().text = "BOSS RUSH";
            bossRushButton.onClick.AddListener(BossRushMenu.Open);
        }
    }

}
