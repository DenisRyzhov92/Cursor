using IdleClickerKit.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClickerKit.UI
{
    public sealed class RewardedAdButtonView : MonoBehaviour
    {
        [SerializeField]
        private RewardedOnlyMonetizationController monetizationController;

        [SerializeField]
        private Button watchAdButton;

        [SerializeField]
        private TMP_Text buttonLabel;

        [SerializeField]
        private TMP_Text statusLabel;

        [SerializeField]
        private string defaultButtonText = "Watch ad for reward";

        private void Awake()
        {
            if (watchAdButton != null)
            {
                watchAdButton.onClick.AddListener(OnWatchAdPressed);
            }
        }

        private void OnDestroy()
        {
            if (watchAdButton != null)
            {
                watchAdButton.onClick.RemoveListener(OnWatchAdPressed);
            }
        }

        private void OnEnable()
        {
            if (monetizationController != null)
            {
                monetizationController.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (monetizationController != null)
            {
                monetizationController.StateChanged -= Refresh;
            }
        }

        private void Update()
        {
            // Cooldown uses timers, so UI is refreshed continuously.
            Refresh();
        }

        public void Refresh()
        {
            if (buttonLabel != null)
            {
                buttonLabel.text = defaultButtonText;
            }

            if (monetizationController == null)
            {
                if (statusLabel != null)
                {
                    statusLabel.text = "Monetization controller missing";
                }

                if (watchAdButton != null)
                {
                    watchAdButton.interactable = false;
                }

                return;
            }

            if (statusLabel != null)
            {
                statusLabel.text = monetizationController.GetStatusText();
            }

            if (watchAdButton != null)
            {
                watchAdButton.interactable = monetizationController.CanShowRewarded;
            }
        }

        private void OnWatchAdPressed()
        {
            if (monetizationController == null)
            {
                return;
            }

            monetizationController.RequestRewardedByUserAction();
        }
    }
}
