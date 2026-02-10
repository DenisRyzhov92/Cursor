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

        [MenuItem("Space Farm/Create Default Config")]
        public static void CreateDefaultConfig()
        {
            EnsureResourcesFolder();

            var existing = AssetDatabase.LoadAssetAtPath<IdleClickerConfig>(ConfigAssetPath);
            if (existing != null)
            {
                Selection.activeObject = existing;
                EditorUtility.DisplayDialog("Space Farm", "Config already exists in Assets/Resources.", "OK");
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
            config.progressBoostOffers = BuildDefaultProgressBoostOffers();
            config.realMoneyProducts = BuildDefaultRealMoneyProducts();

            AssetDatabase.CreateAsset(config, ConfigAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;
            EditorUtility.DisplayDialog("Space Farm", "Default Space Farm config created.", "Great");
        }

        private static List<UpgradeDefinition> BuildDefaultUpgrades()
        {
            return new List<UpgradeDefinition>
            {
                new UpgradeDefinition
                {
                    id = "manual_harvest_protocol",
                    title = "Manual Harvest Protocol",
                    description = "+1 BioGel tap power per level.",
                    maxLevel = 40,
                    unlockAtLifetimeCoins = 0f,
                    baseCost = 10f,
                    costGrowth = 1.18f,
                    clickBonusPerLevel = 1f,
                },
                new UpgradeDefinition
                {
                    id = "micro_drone_swarm",
                    title = "Micro Drone Swarm",
                    description = "+0.4 BioGel/s per level.",
                    maxLevel = 35,
                    unlockAtLifetimeCoins = 15f,
                    baseCost = 25f,
                    costGrowth = 1.2f,
                    passiveBonusPerLevel = 0.4f,
                },
                new UpgradeDefinition
                {
                    id = "hydroponic_racks",
                    title = "Hydroponic Racks",
                    description = "+3 tap and +0.6 BioGel/s per level.",
                    maxLevel = 30,
                    unlockAtLifetimeCoins = 150f,
                    baseCost = 120f,
                    costGrowth = 1.22f,
                    clickBonusPerLevel = 3f,
                    passiveBonusPerLevel = 0.6f,
                },
                new UpgradeDefinition
                {
                    id = "orbital_greenhouse",
                    title = "Orbital Greenhouse",
                    description = "+2 BioGel/s per level.",
                    maxLevel = 25,
                    unlockAtLifetimeCoins = 600f,
                    baseCost = 500f,
                    costGrowth = 1.25f,
                    passiveBonusPerLevel = 2f,
                },
                new UpgradeDefinition
                {
                    id = "solar_mirror_array",
                    title = "Solar Mirror Array",
                    description = "+2% global output per level.",
                    maxLevel = 20,
                    unlockAtLifetimeCoins = 1500f,
                    baseCost = 1000f,
                    costGrowth = 1.27f,
                    globalMultiplierPerLevel = 0.02f,
                },
                new UpgradeDefinition
                {
                    id = "terraforming_ai",
                    title = "Terraforming AI",
                    description = "+10 BioGel/s and +5% output per level.",
                    maxLevel = 15,
                    unlockAtLifetimeCoins = 5000f,
                    baseCost = 4500f,
                    costGrowth = 1.3f,
                    passiveBonusPerLevel = 10f,
                    globalMultiplierPerLevel = 0.05f,
                },
            };
        }

        private static List<ProgressBoostOfferDefinition> BuildDefaultProgressBoostOffers()
        {
            return new List<ProgressBoostOfferDefinition>
            {
                new ProgressBoostOfferDefinition
                {
                    id = "boost_ion_pulse",
                    title = "Ion Pulse Boost",
                    description = "Temporary x1.5 production speed.",
                    unlockAtLifetimeCoins = 40f,
                    bioGelCost = 180f,
                    boostMultiplier = 1.5f,
                    boostDurationSeconds = 180f,
                    instantBioGelReward = 0f,
                },
                new ProgressBoostOfferDefinition
                {
                    id = "boost_orbital_sync",
                    title = "Orbital Sync",
                    description = "x2 production and a small instant drop.",
                    unlockAtLifetimeCoins = 400f,
                    bioGelCost = 1200f,
                    boostMultiplier = 2f,
                    boostDurationSeconds = 300f,
                    instantBioGelReward = 400f,
                },
                new ProgressBoostOfferDefinition
                {
                    id = "boost_terraform_rush",
                    title = "Terraform Rush",
                    description = "Strong late-game boost with instant BioGel.",
                    unlockAtLifetimeCoins = 3000f,
                    bioGelCost = 7000f,
                    boostMultiplier = 3f,
                    boostDurationSeconds = 240f,
                    instantBioGelReward = 2500f,
                },
            };
        }

        private static List<RealMoneyProductDefinition> BuildDefaultRealMoneyProducts()
        {
            return new List<RealMoneyProductDefinition>
            {
                new RealMoneyProductDefinition
                {
                    productId = "iap_starter_supply",
                    title = "Starter Supply Drop",
                    description = "Instant BioGel plus temporary speed boost.",
                    fallbackPriceLabel = "$0.99",
                    bioGelReward = 8000f,
                    progressBoostMultiplier = 1.5f,
                    progressBoostDurationSeconds = 900f,
                },
                new RealMoneyProductDefinition
                {
                    productId = "iap_terraform_booster",
                    title = "Terraform Booster",
                    description = "Powerful temporary multiplier and bonus BioGel.",
                    fallbackPriceLabel = "$2.99",
                    bioGelReward = 20000f,
                    progressBoostMultiplier = 2f,
                    progressBoostDurationSeconds = 3600f,
                },
                new RealMoneyProductDefinition
                {
                    productId = "iap_colony_bundle",
                    title = "Colony Expansion Bundle",
                    description = "Big utility pack for mid-game acceleration.",
                    fallbackPriceLabel = "$4.99",
                    bioGelReward = 70000f,
                    progressBoostMultiplier = 3f,
                    progressBoostDurationSeconds = 7200f,
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
