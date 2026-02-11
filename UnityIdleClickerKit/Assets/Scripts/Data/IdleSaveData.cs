using System;
using System.Collections.Generic;

namespace IdleClickerKit.Data
{
    [Serializable]
    public class IdleSaveData
    {
        public string version = "3";
        public double coins = 0d;
        public double lifetimeCoins = 0d;
        public long lastSaveUnixUtc = 0L;
        public List<UpgradeLevelData> upgradeLevels = new List<UpgradeLevelData>();
    }

    [Serializable]
    public class UpgradeLevelData
    {
        public string id = string.Empty;
        public int level = 0;
    }
}
