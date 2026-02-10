using System;
using UnityEngine;

namespace IdleClickerKit.Config
{
    [Serializable]
    public class BeadExchangeOfferDefinition
    {
        public string id = "bead_offer_id";
        public string title = "Bead Offer";

        [TextArea(2, 4)]
        public string description = "Exchange BioGel for Beads.";

        [Min(0f)]
        public float unlockAtLifetimeCoins = 0f;

        [Min(1f)]
        public float bioGelCost = 100f;

        [Min(1f)]
        public float beadsAmount = 10f;
    }
}
