using BepInEx;
using BossRush.UI;
using HarmonyLib;
using UnityEngine;

namespace BossRush
{
    [BepInPlugin(ConstInfo.GUID, ConstInfo.NAME, ConstInfo.VERSION)]
    public class BossRush : BaseUnityPlugin
    {
        public static BossRush Instance { get; private set; }
        Harmony harmony;

        private bool debug = false;

        private void Awake()
        {
            Instance = this;
            BossRushConfig.Bind();
            Logger.LogInfo("Boss Rush loaded!");
            harmony = new Harmony(ConstInfo.GUID + ".harmony");
            harmony.PatchAll();
            Assets.LoadAssets();
            BossRushStats.Spawn();
        }

        private void Update()
        {
            CheckDebugInputs();

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
        }
    }
}
