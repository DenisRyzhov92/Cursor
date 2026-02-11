using System;
using System.Collections;
using UnityEngine;

namespace IdleClickerKit.Monetization
{
    public sealed class MockIapProvider : IapProviderBase
    {
        [SerializeField]
        private bool initialized = true;

        [SerializeField]
        [Min(0f)]
        private float simulatedDelaySeconds = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float successChance = 1f;

        [SerializeField]
        private string mockLocalizedPrice = "$0.99";

        private bool purchaseInProgress = false;

        public override bool IsInitialized
        {
            get { return initialized; }
        }

        public override bool CanPurchase(string productId)
        {
            return initialized && !purchaseInProgress && !string.IsNullOrWhiteSpace(productId);
        }

        public override void Purchase(string productId, Action<IapPurchaseResult> onCompleted)
        {
            if (!CanPurchase(productId))
            {
                onCompleted?.Invoke(new IapPurchaseResult
                {
                    productId = productId,
                    isSuccess = false,
                    errorMessage = "Mock IAP is not ready.",
                });
                return;
            }

            StartCoroutine(SimulatePurchase(productId, onCompleted));
        }

        public override string GetLocalizedPriceOrFallback(string productId, string fallbackPriceLabel)
        {
            if (!string.IsNullOrWhiteSpace(mockLocalizedPrice))
            {
                return mockLocalizedPrice;
            }

            return base.GetLocalizedPriceOrFallback(productId, fallbackPriceLabel);
        }

        private IEnumerator SimulatePurchase(string productId, Action<IapPurchaseResult> onCompleted)
        {
            purchaseInProgress = true;

            if (simulatedDelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(simulatedDelaySeconds);
            }

            purchaseInProgress = false;
            var success = UnityEngine.Random.value <= successChance;

            onCompleted?.Invoke(new IapPurchaseResult
            {
                productId = productId,
                isSuccess = success,
                errorMessage = success ? string.Empty : "Purchase cancelled or failed in mock flow.",
            });
        }
    }
}
