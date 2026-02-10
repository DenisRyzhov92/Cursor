using System;
using UnityEngine;

namespace IdleClickerKit.Config
{
    [Serializable]
    public class ProgressBoostOfferDefinition
    {
        public string id = "boost_offer_id";
        public string title = "Progress Boost";

        [TextArea(2, 4)]
        public string description = "Temporary speed boost.";

        [Min(0f)]
        public float unlockAtLifetimeCoins = 0f;

        [Min(1f)]
        public float bioGelCost = 100f;

        [Min(1f)]
        [Tooltip("1.5 means +50% progress speed while active.")]
        public float boostMultiplier = 1.5f;

        [Min(1f)]
        public float boostDurationSeconds = 300f;

        [Min(0f)]
        [Tooltip("Optional instant BioGel drop on purchase.")]
        public float instantBioGelReward = 0f;
    }
}
