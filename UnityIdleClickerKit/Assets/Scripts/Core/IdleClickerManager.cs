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
        private bool isInitialized = false;

        private static readonly List<UpgradeSnapshot> EmptySnapshots = new List<UpgradeSnapshot>(0);

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
            get { return engine != null ? engine.ClickPower : 0d; }
        }

        public double PassivePerSecond
        {
            get { return engine != null ? engine.PassivePerSecond : 0d; }
        }

        public double GlobalMultiplier
        {
            get { return engine != null ? engine.GlobalMultiplier : 1d; }
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
            engine.Tick(deltaTime);

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

            engine.Tap();
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

            return new List<UpgradeDefinition>(0);
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
    }
}
