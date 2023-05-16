using UnityEngine;

namespace BossRush
{
    public static class Assets
    {
        public static GameObject BossRushStatsPrefab { get; private set; }
        public static GameObject BossRushMenuPrefab { get; private set; }

        public static GameObject BossRushDeathScreen { get; private set; }

        public static AssetBundle assets;

        public static void LoadAssets()
        {
            assets = AssetBundle.LoadFromMemory(Properties.Resources.BossRush);

            BossRushStatsPrefab = assets.LoadAsset<GameObject>("BossRushStatsDisplay");
            BossRushMenuPrefab = assets.LoadAsset<GameObject>("BossRushMenu");
            BossRushDeathScreen = assets.LoadAsset<GameObject>("BossRushDeathScreen");
        }


    }
}
