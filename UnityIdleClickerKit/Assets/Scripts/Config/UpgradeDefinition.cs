using System;
using UnityEngine;

namespace IdleClickerKit.Config
{
    [Serializable]
    public class UpgradeDefinition
    {
        public string id = "upgrade_id";
        public string title = "Upgrade";

        [TextArea(2, 4)]
        public string description = "Add effect";

        [Min(1)]
        public int maxLevel = 25;

        [Min(0f)]
        public float unlockAtLifetimeCoins = 0f;

        [Min(0.01f)]
        public float baseCost = 10f;

        [Min(1.01f)]
        public float costGrowth = 1.15f;

        public float clickBonusPerLevel = 0f;
        public float passiveBonusPerLevel = 0f;

        [Tooltip("0.01 = +1% global multiplier per level.")]
        public float globalMultiplierPerLevel = 0f;
    }
}
