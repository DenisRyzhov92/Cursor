using System;
using IdleClickerKit.Core;
using UnityEngine;
using UnityEngine.Events;

namespace IdleClickerKit.Monetization
{
    public sealed class RealMoneyStoreController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private IapProviderBase iapProvider;

        [Header("Policy")]
        [SerializeField]
        [Tooltip("Purchases can be initiated only by explicit user action.")]
        private bool requireUserInitiatedPurchase = true;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onPurchaseGranted;

        [SerializeField]
        private UnityEvent onPurchaseRejected;

        private bool purchaseInProgress = false;
        private string processingProductId = string.Empty;

        public event Action StateChanged;

        public bool PurchaseInProgress
        {
            get { return purchaseInProgress; }
        }

        public string ProcessingProductId
        {
            get { return processingProductId; }
        }

        public bool CanPurchaseProduct(string productId)
        {
            if (purchaseInProgress || string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            if (manager == null || !manager.IsReady || iapProvider == null || !iapProvider.IsInitialized)
            {
                return false;
            }

            return iapProvider.CanPurchase(productId);
        }

        public string GetPriceLabel(string productId, string fallbackPriceLabel)
        {
            if (iapProvider == null)
            {
                return string.IsNullOrWhiteSpace(fallbackPriceLabel) ? "Real money" : fallbackPriceLabel;
            }

            return iapProvider.GetLocalizedPriceOrFallback(productId, fallbackPriceLabel);
        }

        public void RequestPurchaseByUserAction(string productId)
        {
            TryPurchase(productId, true);
        }

        public string GetStatusText(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return "Invalid product";
            }

            if (manager == null || !manager.IsReady)
            {
                return "Game manager missing";
            }

            if (iapProvider == null || !iapProvider.IsInitialized)
            {
                return "Store is not ready";
            }

            if (purchaseInProgress)
            {
                return string.Equals(processingProductId, productId, StringComparison.Ordinal)
                    ? "Purchase in progress..."
                    : "Another purchase is in progress";
            }

            if (!iapProvider.CanPurchase(productId))
            {
                return "Product not available";
            }

            return "Ready";
        }

        private void TryPurchase(string productId, bool isUserInitiated)
        {
            if (requireUserInitiatedPurchase && !isUserInitiated)
            {
                return;
            }

            if (!CanPurchaseProduct(productId))
            {
                onPurchaseRejected?.Invoke();
                StateChanged?.Invoke();
                return;
            }

            purchaseInProgress = true;
            processingProductId = productId;
            StateChanged?.Invoke();

            iapProvider.Purchase(productId, result => HandlePurchaseResult(productId, result));
        }

        private void HandlePurchaseResult(string requestedProductId, IapPurchaseResult result)
        {
            purchaseInProgress = false;
            processingProductId = string.Empty;

            var success = result != null &&
                          result.isSuccess &&
                          string.Equals(result.productId, requestedProductId, StringComparison.Ordinal) &&
                          manager != null &&
                          manager.TryApplyRealMoneyProduct(requestedProductId);

            if (success)
            {
                onPurchaseGranted?.Invoke();
            }
            else
            {
                onPurchaseRejected?.Invoke();
            }

            StateChanged?.Invoke();
        }
    }
}
