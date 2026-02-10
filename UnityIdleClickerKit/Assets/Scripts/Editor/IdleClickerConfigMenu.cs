#if UNITY_EDITOR
using System.Collections.Generic;
using IdleClickerKit.Config;
using UnityEditor;
using UnityEngine;

namespace IdleClickerKit.Editor
{
    public static class IdleClickerConfigMenu
    {
        private const string ResourcesFolder = "Assets/Resources";
        private const string ConfigAssetPath = "Assets/Resources/IdleClickerConfig.asset";

        [MenuItem("Idle Clicker/Create Default Config")]
        public static void CreateDefaultConfig()
        {
            EnsureResourcesFolder();

            var existing = AssetDatabase.LoadAssetAtPath<IdleClickerConfig>(ConfigAssetPath);
            if (existing != null)
            {
                Selection.activeObject = existing;
                EditorUtility.DisplayDialog("Idle Clicker", "Config already exists in Assets/Resources.", "OK");
                return;
            }

            var config = ScriptableObject.CreateInstance<IdleClickerConfig>();
            config.startingCoins = 0f;
            config.baseClickPower = 1f;
            config.basePassivePerSecond = 0f;
            config.offlineIncomeEfficiency = 0.7f;
            config.offlineIncomeCapSeconds = 8f * 3600f;
            config.autosaveIntervalSeconds = 8f;
            config.upgrades = BuildDefaultUpgrades();

            AssetDatabase.CreateAsset(config, ConfigAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;
            EditorUtility.DisplayDialog("Idle Clicker", "Default IdleClickerConfig created.", "Great");
        }

        private static List<UpgradeDefinition> BuildDefaultUpgrades()
        {
            return new List<UpgradeDefinition>
            {
                new UpgradeDefinition
                {
                    id = "tap_training",
                    title = "Tap Training",
                    description = "+1 tap power per level.",
                    maxLevel = 40,
                    unlockAtLifetimeCoins = 0f,
                    baseCost = 10f,
                    costGrowth = 1.18f,
                    clickBonusPerLevel = 1f,
                },
                new UpgradeDefinition
                {
                    id = "auto_cursor",
                    title = "Auto Cursor",
                    description = "+0.4 idle/s per level.",
                    maxLevel = 35,
                    unlockAtLifetimeCoins = 15f,
                    baseCost = 25f,
                    costGrowth = 1.2f,
                    passiveBonusPerLevel = 0.4f,
                },
                new UpgradeDefinition
                {
                    id = "iron_tools",
                    title = "Iron Tools",
                    description = "+3 tap and +0.6 idle/s per level.",
                    maxLevel = 30,
                    unlockAtLifetimeCoins = 150f,
                    baseCost = 120f,
                    costGrowth = 1.22f,
                    clickBonusPerLevel = 3f,
                    passiveBonusPerLevel = 0.6f,
                },
                new UpgradeDefinition
                {
                    id = "mini_factory",
                    title = "Mini Factory",
                    description = "+2 idle/s per level.",
                    maxLevel = 25,
                    unlockAtLifetimeCoins = 600f,
                    baseCost = 500f,
                    costGrowth = 1.25f,
                    passiveBonusPerLevel = 2f,
                },
                new UpgradeDefinition
                {
                    id = "ad_campaign",
                    title = "Ad Campaign",
                    description = "+2% global multiplier per level.",
                    maxLevel = 20,
                    unlockAtLifetimeCoins = 1500f,
                    baseCost = 1000f,
                    costGrowth = 1.27f,
                    globalMultiplierPerLevel = 0.02f,
                },
                new UpgradeDefinition
                {
                    id = "ai_manager",
                    title = "AI Manager",
                    description = "+10 idle/s and +5% multiplier per level.",
                    maxLevel = 15,
                    unlockAtLifetimeCoins = 5000f,
                    baseCost = 4500f,
                    costGrowth = 1.3f,
                    passiveBonusPerLevel = 10f,
                    globalMultiplierPerLevel = 0.05f,
                },
            };
        }

        private static void EnsureResourcesFolder()
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
        }
    }
}
#endif
