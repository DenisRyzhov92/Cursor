using IdleClickerKit.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClickerKit.UI
{
    public sealed class BeadOfferButtonView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private string offerId;

        [Header("UI References")]
        [SerializeField]
        private Button buyButton;

        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private TMP_Text valueText;

        [SerializeField]
        private TMP_Text costText;

        [SerializeField]
        private TMP_Text stateText;

        private void Awake()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyPressed);
            }
        }

        private void OnDestroy()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveListener(OnBuyPressed);
            }
        }

        private void OnEnable()
        {
            if (manager != null)
            {
                manager.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (manager != null)
            {
                manager.StateChanged -= Refresh;
            }
        }

        public void Bind(IdleClickerManager sourceManager, string id, string title, string description)
        {
            if (manager != null)
            {
                manager.StateChanged -= Refresh;
            }

            manager = sourceManager;
            offerId = id;

            if (titleText != null)
            {
                titleText.text = title;
            }

            if (descriptionText != null)
            {
                descriptionText.text = description;
            }

            if (manager != null && isActiveAndEnabled)
            {
                manager.StateChanged += Refresh;
            }

            Refresh();
        }

        public void Refresh()
        {
            if (manager == null || !manager.IsReady || string.IsNullOrWhiteSpace(offerId))
            {
                return;
            }

            var snapshot = manager.GetBeadExchangeOfferSnapshot(offerId);
            if (snapshot == null)
            {
                return;
            }

            if (titleText != null && string.IsNullOrWhiteSpace(titleText.text))
            {
                titleText.text = snapshot.title;
            }

            if (descriptionText != null && string.IsNullOrWhiteSpace(descriptionText.text))
            {
                descriptionText.text = snapshot.description;
            }

            if (valueText != null)
            {
                valueText.text = $"+{NumberFormatter.Compact(snapshot.beadsAmount)} Beads";
            }

            if (costText != null)
            {
                costText.text = $"Cost: {NumberFormatter.Compact(snapshot.bioGelCost)} BioGel";
            }

            if (!snapshot.isUnlocked)
            {
                if (stateText != null)
                {
                    stateText.text =
                        $"Unlock at {NumberFormatter.Compact(snapshot.unlockAtLifetimeCoins)} total BioGel";
                }

                if (buyButton != null)
                {
                    buyButton.interactable = false;
                }

                return;
            }

            if (stateText != null)
            {
                stateText.text = snapshot.canBuy ? "Exchange" : "Need more BioGel";
            }

            if (buyButton != null)
            {
                buyButton.interactable = snapshot.canBuy;
            }
        }

        private void OnBuyPressed()
        {
            if (manager == null || string.IsNullOrWhiteSpace(offerId))
            {
                return;
            }

            manager.TryBuyBeadExchangeOffer(offerId);
        }
    }
}
