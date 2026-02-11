using System;
using System.Collections.Generic;
using IdleClickerKit.Config;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace IdleClickerKit.Monetization
{
#if UNITY_PURCHASING
    public sealed class UnityIapProvider : IapProviderBase, IDetailedStoreListener
    {
        [Header("Initialization")]
        [SerializeField]
        private bool autoInitializeOnStart = true;

        [SerializeField]
        private bool includeProductsFromConfig = true;

        [SerializeField]
        private IdleClickerConfig configOverride;

        [SerializeField]
        private string resourcesConfigName = "IdleClickerConfig";

        [SerializeField]
        private bool verboseLogging = false;

        [Header("Catalog")]
        [SerializeField]
        private ProductType defaultProductType = ProductType.Consumable;

        [SerializeField]
        private List<string> additionalProductIds = new List<string>();

        private IStoreController storeController;
        private IExtensionProvider storeExtensions;
        private bool initializationInProgress = false;
        private readonly Dictionary<string, Action<IapPurchaseResult>> pendingPurchaseCallbacks =
            new Dictionary<string, Action<IapPurchaseResult>>(StringComparer.Ordinal);

        public override bool IsInitialized
        {
            get { return storeController != null && storeExtensions != null; }
        }

        private void Start()
        {
            if (autoInitializeOnStart)
            {
                InitializePurchasing();
            }
        }

        private void OnDestroy()
        {
            FailAllPendingPurchases("IAP provider destroyed.");
        }

        [ContextMenu("Initialize Unity IAP")]
        public void InitializePurchasing()
        {
            if (IsInitialized || initializationInProgress)
            {
                return;
            }

            var productIds = CollectProductIds();
            if (productIds.Count == 0)
            {
                Debug.LogWarning("[IdleClicker] UnityIapProvider has no product IDs to initialize.");
                return;
            }

            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);

            for (var i = 0; i < productIds.Count; i++)
            {
                builder.AddProduct(productIds[i], defaultProductType);
            }

            initializationInProgress = true;
            UnityPurchasing.Initialize(this, builder);
        }

        public override bool CanPurchase(string productId)
        {
            if (!IsInitialized || string.IsNullOrWhiteSpace(productId))
            {
                return false;
            }

            if (pendingPurchaseCallbacks.ContainsKey(productId))
            {
                return false;
            }

            var product = storeController.products.WithID(productId);
            return product != null && product.availableToPurchase;
        }

        public override void Purchase(string productId, Action<IapPurchaseResult> onCompleted)
        {
            if (!CanPurchase(productId))
            {
                onCompleted?.Invoke(new IapPurchaseResult
                {
                    productId = productId ?? string.Empty,
                    isSuccess = false,
                    errorMessage = "Product is not available for purchase.",
                });
                return;
            }

            pendingPurchaseCallbacks[productId] = onCompleted;

            try
            {
                storeController.InitiatePurchase(productId);
            }
            catch (Exception exception)
            {
                pendingPurchaseCallbacks.Remove(productId);
                onCompleted?.Invoke(new IapPurchaseResult
                {
                    productId = productId,
                    isSuccess = false,
                    errorMessage = $"Failed to start purchase: {exception.Message}",
                });
            }
        }

        public override string GetLocalizedPriceOrFallback(string productId, string fallbackPriceLabel)
        {
            if (!IsInitialized || string.IsNullOrWhiteSpace(productId))
            {
                return base.GetLocalizedPriceOrFallback(productId, fallbackPriceLabel);
            }

            var product = storeController.products.WithID(productId);
            if (product != null &&
                product.metadata != null &&
                !string.IsNullOrWhiteSpace(product.metadata.localizedPriceString))
            {
                return product.metadata.localizedPriceString;
            }

            return base.GetLocalizedPriceOrFallback(productId, fallbackPriceLabel);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            storeExtensions = extensions;
            initializationInProgress = false;
            LogVerbose("[IdleClicker] Unity IAP initialized.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            initializationInProgress = false;
            storeController = null;
            storeExtensions = null;
            FailAllPendingPurchases($"IAP init failed: {error}");
            Debug.LogError($"[IdleClicker] Unity IAP init failed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            initializationInProgress = false;
            storeController = null;
            storeExtensions = null;
            var errorMessage = $"IAP init failed: {error}. {message}";
            FailAllPendingPurchases(errorMessage);
            Debug.LogError($"[IdleClicker] {errorMessage}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var productId = purchaseEvent?.purchasedProduct?.definition?.id;
            if (string.IsNullOrWhiteSpace(productId))
            {
                return PurchaseProcessingResult.Complete;
            }

            Action<IapPurchaseResult> callback;
            if (!pendingPurchaseCallbacks.TryGetValue(productId, out callback))
            {
                // Safety: ignore purchases not explicitly initiated through this provider.
                LogVerbose($"[IdleClicker] Purchase callback ignored for product {productId}.");
                return PurchaseProcessingResult.Complete;
            }

            pendingPurchaseCallbacks.Remove(productId);
            callback?.Invoke(new IapPurchaseResult
            {
                productId = productId,
                isSuccess = true,
                errorMessage = string.Empty,
            });

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            var productId = product != null && product.definition != null ? product.definition.id : string.Empty;
            FailSinglePurchase(productId, $"Purchase failed: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            var productId = product != null && product.definition != null ? product.definition.id : string.Empty;
            var reason = failureDescription != null
                ? $"{failureDescription.reason}: {failureDescription.message}"
                : "Purchase failed.";
            FailSinglePurchase(productId, reason);
        }

        private List<string> CollectProductIds()
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);

            for (var i = 0; i < additionalProductIds.Count; i++)
            {
                var id = additionalProductIds[i];
                if (!string.IsNullOrWhiteSpace(id))
                {
                    ids.Add(id);
                }
            }

            if (!includeProductsFromConfig)
            {
                return new List<string>(ids);
            }

            var config = configOverride;
            if (config == null && !string.IsNullOrWhiteSpace(resourcesConfigName))
            {
                config = Resources.Load<IdleClickerConfig>(resourcesConfigName);
            }

            if (config == null || config.realMoneyProducts == null)
            {
                return new List<string>(ids);
            }

            for (var i = 0; i < config.realMoneyProducts.Count; i++)
            {
                var definition = config.realMoneyProducts[i];
                if (definition == null || string.IsNullOrWhiteSpace(definition.productId))
                {
                    continue;
                }

                ids.Add(definition.productId);
            }

            return new List<string>(ids);
        }

        private void FailSinglePurchase(string productId, string message)
        {
            Action<IapPurchaseResult> callback;
            if (!pendingPurchaseCallbacks.TryGetValue(productId, out callback))
            {
                LogVerbose($"[IdleClicker] Purchase failure ignored for product {productId}: {message}");
                return;
            }

            pendingPurchaseCallbacks.Remove(productId);
            callback?.Invoke(new IapPurchaseResult
            {
                productId = productId ?? string.Empty,
                isSuccess = false,
                errorMessage = message ?? "Purchase failed.",
            });
        }

        private void FailAllPendingPurchases(string message)
        {
            if (pendingPurchaseCallbacks.Count == 0)
            {
                return;
            }

            var pendingProductIds = new List<string>(pendingPurchaseCallbacks.Keys);
            for (var i = 0; i < pendingProductIds.Count; i++)
            {
                var productId = pendingProductIds[i];
                FailSinglePurchase(productId, message);
            }
        }

        private void LogVerbose(string message)
        {
            if (verboseLogging)
            {
                Debug.Log(message);
            }
        }
    }
#else
    public sealed class UnityIapProvider : IapProviderBase
    {
        [SerializeField]
        private string disabledMessage = "UNITY_PURCHASING is not enabled. Install Unity IAP package first.";

        public override bool IsInitialized
        {
            get { return false; }
        }

        public override bool CanPurchase(string productId)
        {
            return false;
        }

        public override void Purchase(string productId, Action<IapPurchaseResult> onCompleted)
        {
            onCompleted?.Invoke(new IapPurchaseResult
            {
                productId = productId ?? string.Empty,
                isSuccess = false,
                errorMessage = disabledMessage,
            });
        }
    }
#endif
}
