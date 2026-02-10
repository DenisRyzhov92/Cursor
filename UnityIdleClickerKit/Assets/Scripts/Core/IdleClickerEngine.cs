using System;
using System.Collections.Generic;
using IdleClickerKit.Config;
using IdleClickerKit.Data;

namespace IdleClickerKit.Core
{
    public sealed class IdleClickerEngine
    {
        private readonly IdleClickerConfig config;
        private readonly List<UpgradeDefinition> definitions = new List<UpgradeDefinition>();
        private readonly Dictionary<string, UpgradeDefinition> definitionsById =
            new Dictionary<string, UpgradeDefinition>(StringComparer.Ordinal);
        private readonly Dictionary<string, int> levelsById =
            new Dictionary<string, int>(StringComparer.Ordinal);

        private double clickBonusSum = 0d;
        private double passiveBonusSum = 0d;
        private double globalMultiplierBonusSum = 0d;
        private long lastSaveUnixUtc = 0L;

        public IdleClickerEngine(IdleClickerConfig config, IdleSaveData saveData = null)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            BuildDefinitionCache(config.upgrades);
            LoadProgress(saveData);
            RecalculateBonuses();
        }

        public double Coins { get; private set; }
        public double LifetimeCoins { get; private set; }

        public double ClickPower
        {
            get { return (config.baseClickPower + clickBonusSum) * GlobalMultiplier; }
        }

        public double PassivePerSecond
        {
            get { return (config.basePassivePerSecond + passiveBonusSum) * GlobalMultiplier; }
        }

        public double GlobalMultiplier
        {
            get { return 1d + globalMultiplierBonusSum; }
        }

        public IReadOnlyList<UpgradeDefinition> Definitions
        {
            get { return definitions; }
        }

        public IReadOnlyList<UpgradeSnapshot> GetUpgradeSnapshots()
        {
            var snapshots = new List<UpgradeSnapshot>(definitions.Count);
            for (var i = 0; i < definitions.Count; i++)
            {
                snapshots.Add(BuildSnapshot(definitions[i]));
            }

            return snapshots;
        }

        public UpgradeSnapshot GetUpgradeSnapshot(string upgradeId)
        {
            UpgradeDefinition definition;
            if (string.IsNullOrEmpty(upgradeId) || !definitionsById.TryGetValue(upgradeId, out definition))
            {
                return null;
            }

            return BuildSnapshot(definition);
        }

        public void Tap()
        {
            AddCoins(ClickPower);
        }

        public void GrantCoins(double amount)
        {
            AddCoins(amount);
        }

        public void Tick(double deltaTimeSeconds)
        {
            if (deltaTimeSeconds <= 0d)
            {
                return;
            }

            AddCoins(PassivePerSecond * deltaTimeSeconds);
        }

        public bool TryBuyUpgrade(string upgradeId)
        {
            UpgradeDefinition definition;
            if (string.IsNullOrEmpty(upgradeId) || !definitionsById.TryGetValue(upgradeId, out definition))
            {
                return false;
            }

            var level = levelsById[upgradeId];
            if (level >= definition.maxLevel)
            {
                return false;
            }

            if (!IsUpgradeUnlocked(definition, level))
            {
                return false;
            }

            var cost = GetUpgradeCost(definition, level);
            if (Coins + 1e-9d < cost)
            {
                return false;
            }

            Coins -= cost;
            levelsById[upgradeId] = level + 1;
            RecalculateBonuses();
            return true;
        }

        public OfflineProgressResult ApplyOfflineProgress(long nowUnixUtc, float capSeconds, float efficiency)
        {
            var result = new OfflineProgressResult();

            if (nowUnixUtc <= 0L)
            {
                nowUnixUtc = GetUnixUtcNow();
            }

            if (lastSaveUnixUtc <= 0L)
            {
                lastSaveUnixUtc = nowUnixUtc;
                return result;
            }

            var elapsed = Math.Max(0d, nowUnixUtc - lastSaveUnixUtc);
            var cap = Math.Max(0d, capSeconds);
            var simulated = Math.Min(elapsed, cap);
            var normalizedEfficiency = Clamp01(efficiency);
            var earned = PassivePerSecond * simulated * normalizedEfficiency;

            AddCoins(earned);
            lastSaveUnixUtc = nowUnixUtc;

            result.elapsedSeconds = elapsed;
            result.simulatedSeconds = simulated;
            result.earnedCoins = earned;
            return result;
        }

        public IdleSaveData BuildSaveData()
        {
            var saveData = new IdleSaveData();
            saveData.coins = Coins;
            saveData.lifetimeCoins = LifetimeCoins;
            saveData.lastSaveUnixUtc = GetUnixUtcNow();
            saveData.upgradeLevels = new List<UpgradeLevelData>(definitions.Count);

            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                saveData.upgradeLevels.Add(new UpgradeLevelData
                {
                    id = definition.id,
                    level = levelsById[definition.id],
                });
            }

            lastSaveUnixUtc = saveData.lastSaveUnixUtc;
            return saveData;
        }

        public void ResetProgress()
        {
            Coins = Math.Max(0d, config.startingCoins);
            LifetimeCoins = Coins;

            for (var i = 0; i < definitions.Count; i++)
            {
                levelsById[definitions[i].id] = 0;
            }

            RecalculateBonuses();
            lastSaveUnixUtc = GetUnixUtcNow();
        }

        private void AddCoins(double amount)
        {
            if (amount <= 0d || double.IsNaN(amount) || double.IsInfinity(amount))
            {
                return;
            }

            Coins += amount;
            LifetimeCoins += amount;
        }

        private UpgradeSnapshot BuildSnapshot(UpgradeDefinition definition)
        {
            var currentLevel = levelsById[definition.id];
            var isMaxLevel = currentLevel >= definition.maxLevel;
            var unlocked = IsUpgradeUnlocked(definition, currentLevel);
            var cost = isMaxLevel ? 0d : GetUpgradeCost(definition, currentLevel);
            var canBuy = unlocked && !isMaxLevel && Coins + 1e-9d >= cost;

            return new UpgradeSnapshot
            {
                id = definition.id,
                title = definition.title,
                description = definition.description,
                level = currentLevel,
                maxLevel = definition.maxLevel,
                cost = cost,
                unlockAtLifetimeCoins = definition.unlockAtLifetimeCoins,
                isUnlocked = unlocked,
                isMaxLevel = isMaxLevel,
                canBuy = canBuy,
            };
        }

        private static double GetUpgradeCost(UpgradeDefinition definition, int level)
        {
            var safeBaseCost = Math.Max(0.01d, definition.baseCost);
            var safeGrowth = Math.Max(1.01d, definition.costGrowth);
            return safeBaseCost * Math.Pow(safeGrowth, level);
        }

        private bool IsUpgradeUnlocked(UpgradeDefinition definition, int currentLevel)
        {
            return currentLevel > 0 || LifetimeCoins >= definition.unlockAtLifetimeCoins;
        }

        private void RecalculateBonuses()
        {
            clickBonusSum = 0d;
            passiveBonusSum = 0d;
            globalMultiplierBonusSum = 0d;

            for (var i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                var level = levelsById[definition.id];

                clickBonusSum += definition.clickBonusPerLevel * level;
                passiveBonusSum += definition.passiveBonusPerLevel * level;
                globalMultiplierBonusSum += definition.globalMultiplierPerLevel * level;
            }

            if (globalMultiplierBonusSum < 0d)
            {
                globalMultiplierBonusSum = 0d;
            }
        }

        private void BuildDefinitionCache(List<UpgradeDefinition> inputDefinitions)
        {
            if (inputDefinitions == null)
            {
                return;
            }

            for (var i = 0; i < inputDefinitions.Count; i++)
            {
                var definition = inputDefinitions[i];
                if (definition == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(definition.id) || definitionsById.ContainsKey(definition.id))
                {
                    continue;
                }

                definitions.Add(definition);
                definitionsById.Add(definition.id, definition);
                levelsById.Add(definition.id, 0);
            }
        }

        private void LoadProgress(IdleSaveData saveData)
        {
            if (saveData == null)
            {
                Coins = Math.Max(0d, config.startingCoins);
                LifetimeCoins = Coins;
                lastSaveUnixUtc = GetUnixUtcNow();
                return;
            }

            Coins = Math.Max(0d, saveData.coins);
            LifetimeCoins = Math.Max(Coins, saveData.lifetimeCoins);
            lastSaveUnixUtc = saveData.lastSaveUnixUtc > 0L ? saveData.lastSaveUnixUtc : GetUnixUtcNow();

            if (saveData.upgradeLevels == null)
            {
                return;
            }

            for (var i = 0; i < saveData.upgradeLevels.Count; i++)
            {
                var savedLevel = saveData.upgradeLevels[i];
                if (savedLevel == null || string.IsNullOrWhiteSpace(savedLevel.id))
                {
                    continue;
                }

                UpgradeDefinition definition;
                if (!definitionsById.TryGetValue(savedLevel.id, out definition))
                {
                    continue;
                }

                var clampedLevel = Math.Max(0, Math.Min(savedLevel.level, definition.maxLevel));
                levelsById[savedLevel.id] = clampedLevel;
            }
        }

        private static long GetUnixUtcNow()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private static double Clamp01(float value)
        {
            if (value <= 0f)
            {
                return 0d;
            }

            if (value >= 1f)
            {
                return 1d;
            }

            return value;
        }
    }
}
