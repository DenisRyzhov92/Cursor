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
            config.startingBeads = 0f;
            config.baseClickPower = 1f;
            config.basePassivePerSecond = 0f;
            config.offlineIncomeEfficiency = 0.7f;
            config.offlineIncomeCapSeconds = 8f * 3600f;
            config.autosaveIntervalSeconds = 8f;
            config.upgrades = BuildDefaultUpgrades();
            config.beadExchangeOffers = BuildDefaultBeadOffers();
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

        private static List<BeadExchangeOfferDefinition> BuildDefaultBeadOffers()
        {
            return new List<BeadExchangeOfferDefinition>
            {
                new BeadExchangeOfferDefinition
                {
                    id = "bead_micro_cache",
                    title = "Micro Bead Cache",
                    description = "Exchange BioGel for a small bundle of Beads.",
                    unlockAtLifetimeCoins = 40f,
                    bioGelCost = 150f,
                    beadsAmount = 10f,
                },
                new BeadExchangeOfferDefinition
                {
                    id = "bead_drone_crate",
                    title = "Drone Bead Crate",
                    description = "Exchange BioGel for a medium Bead crate.",
                    unlockAtLifetimeCoins = 400f,
                    bioGelCost = 900f,
                    beadsAmount = 70f,
                },
                new BeadExchangeOfferDefinition
                {
                    id = "bead_orbital_vault",
                    title = "Orbital Bead Vault",
                    description = "High-tier exchange with best Bead efficiency.",
                    unlockAtLifetimeCoins = 3000f,
                    bioGelCost = 5000f,
                    beadsAmount = 450f,
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
                    description = "Instant BioGel and Beads to speed up early progress.",
                    fallbackPriceLabel = "$0.99",
                    bioGelReward = 8000f,
                    beadsReward = 80f,
                    rewardBoostMultiplier = 1f,
                    rewardBoostDurationSeconds = 0f,
                },
                new RealMoneyProductDefinition
                {
                    productId = "iap_terraform_booster",
                    title = "Terraform Booster",
                    description = "Powerful temporary multiplier and bonus resources.",
                    fallbackPriceLabel = "$2.99",
                    bioGelReward = 20000f,
                    beadsReward = 150f,
                    rewardBoostMultiplier = 2f,
                    rewardBoostDurationSeconds = 3600f,
                },
                new RealMoneyProductDefinition
                {
                    productId = "iap_colony_bundle",
                    title = "Colony Expansion Bundle",
                    description = "Big utility pack for mid-game acceleration.",
                    fallbackPriceLabel = "$4.99",
                    bioGelReward = 70000f,
                    beadsReward = 600f,
                    rewardBoostMultiplier = 2f,
                    rewardBoostDurationSeconds = 7200f,
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
