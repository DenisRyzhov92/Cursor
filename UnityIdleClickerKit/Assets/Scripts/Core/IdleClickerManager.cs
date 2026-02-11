using System;
using System.Collections.Generic;
using IdleClickerKit.Config;
using IdleClickerKit.Data;
using UnityEngine;

namespace IdleClickerKit.Core
{
    public sealed class IdleClickerManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private IdleClickerConfig config;

        [SerializeField]
        private string saveFileName = "idle_clicker_save.json";

        [Header("Startup")]
        [SerializeField]
        private bool autoInitialize = true;

        [SerializeField]
        private bool loadFromDiskOnStart = true;

        [SerializeField]
        private bool applyOfflineProgressOnStart = true;

        [Header("Runtime")]
        [SerializeField]
        private bool autosaveEnabled = true;

        private IdleClickerEngine engine;
        private float autosaveTimer = 0f;
        private float rewardBoostSecondsRemaining = 0f;
        private double rewardBoostMultiplier = 1d;
        private bool isInitialized = false;

        private static readonly List<UpgradeSnapshot> EmptySnapshots = new List<UpgradeSnapshot>(0);
        private static readonly List<ProgressBoostOfferSnapshot> EmptyProgressBoostOfferSnapshots =
            new List<ProgressBoostOfferSnapshot>(0);
        private static readonly List<UpgradeDefinition> EmptyUpgradeDefinitions = new List<UpgradeDefinition>(0);
        private static readonly List<ProgressBoostOfferDefinition> EmptyProgressBoostOfferDefinitions =
            new List<ProgressBoostOfferDefinition>(0);
        private static readonly List<RealMoneyProductDefinition> EmptyRealMoneyDefinitions =
            new List<RealMoneyProductDefinition>(0);

        public event Action StateChanged;
        public event Action<OfflineProgressResult> OfflineProgressApplied;

        public bool IsReady
        {
            get { return isInitialized; }
        }

        public double Coins
        {
            get { return engine != null ? engine.Coins : 0d; }
        }

        public double LifetimeCoins
        {
            get { return engine != null ? engine.LifetimeCoins : 0d; }
        }

        public double ClickPower
        {
            get { return (engine != null ? engine.ClickPower : 0d) * ActiveRewardBoostMultiplier; }
        }

        public double PassivePerSecond
        {
            get { return (engine != null ? engine.PassivePerSecond : 0d) * ActiveRewardBoostMultiplier; }
        }

        public double GlobalMultiplier
        {
            get { return engine != null ? engine.GlobalMultiplier : 1d; }
        }

        public bool HasActiveRewardBoost
        {
            get { return rewardBoostSecondsRemaining > 0f && rewardBoostMultiplier > 1d; }
        }

        public float ActiveRewardBoostSecondsRemaining
        {
            get { return Mathf.Max(0f, rewardBoostSecondsRemaining); }
        }

        public double ActiveRewardBoostMultiplier
        {
            get { return HasActiveRewardBoost ? rewardBoostMultiplier : 1d; }
        }

        private void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }

        private void Update()
        {
            if (!isInitialized || engine == null)
            {
                return;
            }

            var deltaTime = Time.unscaledDeltaTime;
            TickRewardBoost(deltaTime);
            engine.GrantCoins(PassivePerSecond * deltaTime);

            if (autosaveEnabled && config != null)
            {
                autosaveTimer += deltaTime;
                if (autosaveTimer >= config.autosaveIntervalSeconds)
                {
                    autosaveTimer = 0f;
                    Save();
                }
            }

            StateChanged?.Invoke();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            if (config == null)
            {
                config = Resources.Load<IdleClickerConfig>("IdleClickerConfig");
            }

            if (config == null)
            {
                Debug.LogError("[IdleClicker] Config is missing. Assign IdleClickerConfig in the inspector.");
                enabled = false;
                return;
            }

            IdleSaveData loadedSave = null;
            if (loadFromDiskOnStart)
            {
                loadedSave = IdleSaveStorage.Load(saveFileName);
            }

            engine = new IdleClickerEngine(config, loadedSave);
            isInitialized = true;
            autosaveTimer = 0f;

            if (applyOfflineProgressOnStart && loadedSave != null)
            {
                var result = engine.ApplyOfflineProgress(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    config.offlineIncomeCapSeconds,
                    config.offlineIncomeEfficiency
                );

                if (result.earnedCoins > 0.0001d)
                {
                    OfflineProgressApplied?.Invoke(result);
                }

                // Persist immediately so offline reward cannot be claimed again after force close.
                Save();
            }

            StateChanged?.Invoke();
        }

        public void Tap()
        {
            if (!TryEnsureInitialized())
            {
                return;
            }

            engine.GrantCoins(ClickPower);
            StateChanged?.Invoke();
        }

        public void GrantRewardCoins(double amount)
        {
            if (!TryEnsureInitialized())
            {
                return;
            }

            if (amount <= 0d)
            {
                return;
            }

            engine.GrantCoins(amount);
            StateChanged?.Invoke();
        }

        public void ActivateRewardBoost(float durationSeconds, float multiplier)
        {
            if (!TryEnsureInitialized())
            {
                return;
            }

            if (durationSeconds <= 0f || multiplier <= 1f)
            {
                return;
            }

            ActivateProgressBoost(multiplier, durationSeconds);
            StateChanged?.Invoke();
        }

        public bool TryBuyUpgrade(string upgradeId)
        {
            if (!TryEnsureInitialized())
            {
                return false;
            }

            var wasBought = engine.TryBuyUpgrade(upgradeId);
            if (wasBought)
            {
                StateChanged?.Invoke();
            }

            return wasBought;
        }

        public bool TryBuyProgressBoostOffer(string offerId)
        {
            if (!TryEnsureInitialized())
            {
                return false;
            }

            var definition = FindProgressBoostOfferDefinition(offerId);
            if (definition == null)
            {
                return false;
            }

            var bought = engine.TryBuyProgressBoostOffer(offerId);
            if (!bought)
            {
                return false;
            }

            if (definition.instantBioGelReward > 0f)
            {
                engine.GrantCoins(definition.instantBioGelReward);
            }

            if (definition.boostMultiplier > 1f && definition.boostDurationSeconds > 0f)
            {
                ActivateProgressBoost(definition.boostMultiplier, definition.boostDurationSeconds);
            }

            StateChanged?.Invoke();
            return true;
        }

        public bool TryApplyRealMoneyProduct(string productId)
        {
            if (!TryEnsureInitialized())
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(productId) || config == null || config.realMoneyProducts == null)
            {
                return false;
            }

            var definition = FindRealMoneyProductDefinition(productId);

            if (definition == null)
            {
                return false;
            }

            var anyRewardApplied = false;

            if (definition.bioGelReward > 0f)
            {
                engine.GrantCoins(definition.bioGelReward);
                anyRewardApplied = true;
            }

            if (definition.progressBoostMultiplier > 1f && definition.progressBoostDurationSeconds > 0f)
            {
                ActivateProgressBoost(definition.progressBoostMultiplier, definition.progressBoostDurationSeconds);
                anyRewardApplied = true;
            }

            if (anyRewardApplied)
            {
                StateChanged?.Invoke();
            }

            return anyRewardApplied;
        }

        public UpgradeSnapshot GetUpgradeSnapshot(string upgradeId)
        {
            if (!TryEnsureInitialized())
            {
                return null;
            }

            return engine.GetUpgradeSnapshot(upgradeId);
        }

        public IReadOnlyList<UpgradeSnapshot> GetUpgradeSnapshots()
        {
            if (!TryEnsureInitialized())
            {
                return EmptySnapshots;
            }

            return engine.GetUpgradeSnapshots();
        }

        public ProgressBoostOfferSnapshot GetProgressBoostOfferSnapshot(string offerId)
        {
            if (!TryEnsureInitialized())
            {
                return null;
            }

            return engine.GetProgressBoostOfferSnapshot(offerId);
        }

        public IReadOnlyList<ProgressBoostOfferSnapshot> GetProgressBoostOfferSnapshots()
        {
            if (!TryEnsureInitialized())
            {
                return EmptyProgressBoostOfferSnapshots;
            }

            return engine.GetProgressBoostOfferSnapshots();
        }

        public IReadOnlyList<UpgradeDefinition> GetUpgradeDefinitions()
        {
            if (engine != null)
            {
                return engine.Definitions;
            }

            if (config != null && config.upgrades != null)
            {
                return config.upgrades;
            }

            return EmptyUpgradeDefinitions;
        }

        public IReadOnlyList<ProgressBoostOfferDefinition> GetProgressBoostOfferDefinitions()
        {
            if (engine != null)
            {
                return engine.ProgressBoostOffers;
            }

            if (config != null && config.progressBoostOffers != null)
            {
                return config.progressBoostOffers;
            }

            return EmptyProgressBoostOfferDefinitions;
        }

        public IReadOnlyList<RealMoneyProductDefinition> GetRealMoneyProductDefinitions()
        {
            if (config != null && config.realMoneyProducts != null)
            {
                return config.realMoneyProducts;
            }

            return EmptyRealMoneyDefinitions;
        }

        public string GetSavePath()
        {
            return IdleSaveStorage.GetPath(saveFileName);
        }

        public void Save()
        {
            if (!isInitialized || engine == null)
            {
                return;
            }

            var saveData = engine.BuildSaveData();
            IdleSaveStorage.Save(saveData, saveFileName);
        }

        public void ResetProgressAndWipeSave()
        {
            if (!TryEnsureInitialized())
            {
                return;
            }

            engine.ResetProgress();
            rewardBoostSecondsRemaining = 0f;
            rewardBoostMultiplier = 1d;
            IdleSaveStorage.Delete(saveFileName);
            Save();
            StateChanged?.Invoke();
        }

        private bool TryEnsureInitialized()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            return isInitialized && engine != null;
        }

        private void TickRewardBoost(float deltaTime)
        {
            if (deltaTime <= 0f || !HasActiveRewardBoost)
            {
                return;
            }

            rewardBoostSecondsRemaining = Mathf.Max(0f, rewardBoostSecondsRemaining - deltaTime);
            if (rewardBoostSecondsRemaining <= 0f)
            {
                rewardBoostMultiplier = 1d;
            }
        }

        private void ActivateProgressBoost(float multiplier, float durationSeconds)
        {
            rewardBoostMultiplier = Math.Max(rewardBoostMultiplier, multiplier);
            rewardBoostSecondsRemaining += durationSeconds;
        }

        private ProgressBoostOfferDefinition FindProgressBoostOfferDefinition(string offerId)
        {
            if (string.IsNullOrWhiteSpace(offerId) || config == null || config.progressBoostOffers == null)
            {
                return null;
            }

            for (var i = 0; i < config.progressBoostOffers.Count; i++)
            {
                var definition = config.progressBoostOffers[i];
                if (definition == null)
                {
                    continue;
                }

                if (string.Equals(definition.id, offerId, StringComparison.Ordinal))
                {
                    return definition;
                }
            }

            return null;
        }

        private RealMoneyProductDefinition FindRealMoneyProductDefinition(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId) || config == null || config.realMoneyProducts == null)
            {
                return null;
            }

            for (var i = 0; i < config.realMoneyProducts.Count; i++)
            {
                var candidate = config.realMoneyProducts[i];
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.productId, productId, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return null;
        }
    }
}
