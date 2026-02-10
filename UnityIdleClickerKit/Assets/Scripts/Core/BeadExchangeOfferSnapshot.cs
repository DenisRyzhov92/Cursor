namespace IdleClickerKit.Core
{
    public sealed class BeadExchangeOfferSnapshot
    {
        public string id = string.Empty;
        public string title = string.Empty;
        public string description = string.Empty;
        public double bioGelCost = 0d;
        public double beadsAmount = 0d;
        public double unlockAtLifetimeCoins = 0d;
        public bool isUnlocked = false;
        public bool canBuy = false;
    }
}
