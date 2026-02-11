namespace IdleClickerKit.Core
{
    public sealed class ProgressBoostOfferSnapshot
    {
        public string id = string.Empty;
        public string title = string.Empty;
        public string description = string.Empty;
        public double bioGelCost = 0d;
        public double boostMultiplier = 1d;
        public double boostDurationSeconds = 0d;
        public double instantBioGelReward = 0d;
        public double unlockAtLifetimeCoins = 0d;
        public bool isUnlocked = false;
        public bool canBuy = false;
    }
}
