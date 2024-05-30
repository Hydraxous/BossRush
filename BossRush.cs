using BepInEx;
using BepInEx.Logging;
using BossRush.UI;
using HarmonyLib;
using UnityEngine;

namespace BossRush
{
    [BepInPlugin(ConstInfo.GUID, ConstInfo.NAME, ConstInfo.VERSION)]
    public class BossRush : BaseUnityPlugin
    {
        public static BossRush Instance { get; private set; }
        public static ManualLogSource BepInExLogger => Instance.Logger;
        public static bool LatestVersion { get; private set; }
        public static string VersionName { get; private set; }
        Harmony harmony;

        private bool debug = true;

        private void Awake()
        {
            Instance = this;
            BossRushConfig.Bind();

            BossRushPaths.CheckFolders();
            LevelChainManager.LoadLevelChainTable();
            StatRecords.LoadRecords();

            harmony = new Harmony(ConstInfo.GUID + ".harmony");
            harmony.PatchAll();
            Assets.LoadAssets();
            BossRushStats.Spawn();

            VersionCheck.CheckVersion(ConstInfo.GITHUB_URL, ConstInfo.VERSION, VersionCheckCallback);

            Logger.LogInfo($"Boss Rush {ConstInfo.VERSION} loaded!");
        }

        private void Update()
        {
            CheckDebugInputs();
        }

        private void VersionCheckCallback(bool onLatest, string versionName)
        {
            LatestVersion = onLatest;
            VersionName = versionName;
            if (versionName == "UNKNOWN")
            {
                LatestVersion = true;
                VersionName = ConstInfo.VERSION;
            }

            if (LatestVersion)
                Debug.Log($"BOSSRUSH: You are using the latest version of BossRush ({VersionName}).");
            else
                Debug.LogWarning($"BOSSRUSH: You are using an outdated version of BossRush ({ConstInfo.VERSION}). Please consider updating to {VersionName}");

        }

        private void CheckDebugInputs()
        {
            if (!Input.GetKey(KeyCode.LeftShift) || !debug)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1) && !BossRushController.BossRushMode)
            {
                BossRushController.StartBossRushMode();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && !BossRushController.BossRushMode)
            {
                BossRushController.StartBossRushMode(true);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                BossRushController.HardcoreMode = !BossRushController.HardcoreMode;
                Debug.Log($"BR: Hardcore={BossRushController.HardcoreMode}");
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                BossRushController.BossRushMode = !BossRushController.BossRushMode;
                Debug.Log($"BR: Enabled={BossRushController.BossRushMode}");
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                BossRushController.Laps--;
                Debug.Log($"BR: Laps={BossRushController.Laps}");
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                BossRushController.Laps++;
                Debug.Log($"BR: Laps={BossRushController.Laps}");
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                BossRushController.Laps = 0;
                Debug.Log($"BR: Laps={BossRushController.Laps}");
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                StatRecords.SubmitRecord(BossRushController.GetCurrentStat());
                Debug.Log($"BR: Laps={BossRushController.Laps}");
            }
        }

        private void OnApplicationQuit()
        {
            StatRecords.Save();
        }
    }
}
