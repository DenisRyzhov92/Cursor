using System;
using UnityEngine;

namespace IdleClickerKit.Monetization
{
    public abstract class IapProviderBase : MonoBehaviour
    {
        public abstract bool IsInitialized { get; }
        public abstract bool CanPurchase(string productId);
        public abstract void Purchase(string productId, Action<IapPurchaseResult> onCompleted);

        public virtual string GetLocalizedPriceOrFallback(string productId, string fallbackPriceLabel)
        {
            return string.IsNullOrWhiteSpace(fallbackPriceLabel) ? "Real money" : fallbackPriceLabel;
        }
    }
}
