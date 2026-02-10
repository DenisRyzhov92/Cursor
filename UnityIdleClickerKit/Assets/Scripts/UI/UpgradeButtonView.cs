using IdleClickerKit.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClickerKit.UI
{
    public sealed class UpgradeButtonView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private string upgradeId;

        [Header("UI References")]
        [SerializeField]
        private Button buyButton;

        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private TMP_Text levelText;

        [SerializeField]
        private TMP_Text costText;

        [SerializeField]
        private TMP_Text stateText;

        private void Awake()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveListener(OnBuyButtonClicked);
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
            upgradeId = id;

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
            if (manager == null || !manager.IsReady || string.IsNullOrWhiteSpace(upgradeId))
            {
                return;
            }

            var snapshot = manager.GetUpgradeSnapshot(upgradeId);
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

            if (levelText != null)
            {
                levelText.text = $"Lv {snapshot.level}/{snapshot.maxLevel}";
            }

            if (snapshot.isMaxLevel)
            {
                if (costText != null)
                {
                    costText.text = "MAX";
                }

                if (stateText != null)
                {
                    stateText.text = "Fully upgraded";
                }

                if (buyButton != null)
                {
                    buyButton.interactable = false;
                }

                return;
            }

            if (costText != null)
            {
                costText.text = $"Cost: {NumberFormatter.Compact(snapshot.cost)} BioGel";
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
                stateText.text = snapshot.canBuy ? "Available" : "Need more BioGel";
            }

            if (buyButton != null)
            {
                buyButton.interactable = snapshot.canBuy;
            }
        }

        private void OnBuyButtonClicked()
        {
            if (manager == null || string.IsNullOrWhiteSpace(upgradeId))
            {
                return;
            }

            manager.TryBuyUpgrade(upgradeId);
        }
    }
}
