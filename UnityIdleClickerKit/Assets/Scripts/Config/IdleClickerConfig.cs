using System.Collections.Generic;
using UnityEngine;

namespace IdleClickerKit.Config
{
    [CreateAssetMenu(menuName = "Idle Clicker/Config", fileName = "IdleClickerConfig")]
    public class IdleClickerConfig : ScriptableObject
    {
        [Header("Starting Values")]
        [Min(0f)]
        public float startingCoins = 0f;

        [Min(0f)]
        public float startingBeads = 0f;

        [Min(0f)]
        public float baseClickPower = 1f;

        [Min(0f)]
        public float basePassivePerSecond = 0f;

        [Header("Offline Progress")]
        [Range(0f, 1f)]
        public float offlineIncomeEfficiency = 0.7f;

        [Min(0f)]
        public float offlineIncomeCapSeconds = 28800f;

        [Header("Save")]
        [Min(1f)]
        public float autosaveIntervalSeconds = 8f;

        [Header("Economy")]
        public List<UpgradeDefinition> upgrades = new List<UpgradeDefinition>();

        [Header("Shop: BioGel -> Beads")]
        public List<BeadExchangeOfferDefinition> beadExchangeOffers = new List<BeadExchangeOfferDefinition>();

        [Header("Shop: Real Money Products")]
        public List<RealMoneyProductDefinition> realMoneyProducts = new List<RealMoneyProductDefinition>();
    }
}
