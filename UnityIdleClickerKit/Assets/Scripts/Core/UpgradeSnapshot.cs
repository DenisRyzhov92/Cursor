namespace IdleClickerKit.Core
{
    public sealed class UpgradeSnapshot
    {
        public string id = string.Empty;
        public string title = string.Empty;
        public string description = string.Empty;
        public int level = 0;
        public int maxLevel = 0;
        public double cost = 0d;
        public double unlockAtLifetimeCoins = 0d;
        public bool isUnlocked = false;
        public bool isMaxLevel = false;
        public bool canBuy = false;
    }
}
