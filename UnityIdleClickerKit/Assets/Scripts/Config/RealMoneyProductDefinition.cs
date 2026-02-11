using System;
using UnityEngine;

namespace IdleClickerKit.Config
{
    [Serializable]
    public class RealMoneyProductDefinition
    {
        public string productId = "iap_product_id";
        public string title = "Starter Pack";

        [TextArea(2, 4)]
        public string description = "Useful bundle";

        [Tooltip("Used only in mock/demo UI. Real provider should show localized price.")]
        public string fallbackPriceLabel = "$0.99";

        [Min(0f)]
        public float bioGelReward = 0f;

        [Min(0f)]
        public float progressBoostMultiplier = 1f;

        [Min(0f)]
        public float progressBoostDurationSeconds = 0f;
    }
}
