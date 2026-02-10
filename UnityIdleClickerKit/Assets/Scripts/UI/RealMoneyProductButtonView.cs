using IdleClickerKit.Config;
using IdleClickerKit.Core;
using IdleClickerKit.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClickerKit.UI
{
    public sealed class RealMoneyProductButtonView : MonoBehaviour
    {
        [SerializeField]
        private RealMoneyStoreController storeController;

        [SerializeField]
        private string productId;

        [SerializeField]
        private string fallbackPriceLabel = "$0.99";

        [Header("UI References")]
        [SerializeField]
        private Button purchaseButton;

        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private TMP_Text valueText;

        [SerializeField]
        private TMP_Text priceText;

        [SerializeField]
        private TMP_Text stateText;

        private void Awake()
        {
            if (purchaseButton != null)
            {
                purchaseButton.onClick.AddListener(OnPurchasePressed);
            }
        }

        private void OnDestroy()
        {
            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveListener(OnPurchasePressed);
            }
        }

        private void OnEnable()
        {
            if (storeController != null)
            {
                storeController.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (storeController != null)
            {
                storeController.StateChanged -= Refresh;
            }
        }

        public void Bind(RealMoneyStoreController controller, RealMoneyProductDefinition definition)
        {
            if (storeController != null)
            {
                storeController.StateChanged -= Refresh;
            }

            storeController = controller;
            productId = definition != null ? definition.productId : string.Empty;
            fallbackPriceLabel = definition != null ? definition.fallbackPriceLabel : "$0.99";

            if (titleText != null)
            {
                titleText.text = definition != null ? definition.title : "Product";
            }

            if (descriptionText != null)
            {
                descriptionText.text = definition != null ? definition.description : string.Empty;
            }

            if (valueText != null && definition != null)
            {
                var reward = BuildRewardText(definition);
                valueText.text = reward;
            }

            if (storeController != null && isActiveAndEnabled)
            {
                storeController.StateChanged += Refresh;
            }

            Refresh();
        }

        public void Refresh()
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            if (storeController == null)
            {
                if (stateText != null)
                {
                    stateText.text = "Store controller missing";
                }

                if (purchaseButton != null)
                {
                    purchaseButton.interactable = false;
                }

                return;
            }

            if (priceText != null)
            {
                priceText.text = storeController.GetPriceLabel(productId, fallbackPriceLabel);
            }

            if (stateText != null)
            {
                stateText.text = storeController.GetStatusText(productId);
            }

            if (purchaseButton != null)
            {
                purchaseButton.interactable = storeController.CanPurchaseProduct(productId);
            }
        }

        private void OnPurchasePressed()
        {
            if (storeController == null || string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            storeController.RequestPurchaseByUserAction(productId);
        }

        private static string BuildRewardText(RealMoneyProductDefinition definition)
        {
            var rewardText = string.Empty;

            if (definition.bioGelReward > 0f)
            {
                rewardText += $"+{NumberFormatter.Compact(definition.bioGelReward)} BioGel";
            }

            if (definition.beadsReward > 0f)
            {
                if (!string.IsNullOrEmpty(rewardText))
                {
                    rewardText += " • ";
                }

                rewardText += $"+{NumberFormatter.Compact(definition.beadsReward)} Beads";
            }

            if (definition.rewardBoostMultiplier > 1f && definition.rewardBoostDurationSeconds > 0f)
            {
                if (!string.IsNullOrEmpty(rewardText))
                {
                    rewardText += " • ";
                }

                rewardText +=
                    $"x{definition.rewardBoostMultiplier:0.##} for {definition.rewardBoostDurationSeconds:0}s";
            }

            return string.IsNullOrEmpty(rewardText) ? "Utility pack" : rewardText;
        }
    }
}
