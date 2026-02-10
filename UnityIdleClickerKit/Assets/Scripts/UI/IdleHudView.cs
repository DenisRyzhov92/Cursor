using IdleClickerKit.Core;
using TMPro;
using UnityEngine;

namespace IdleClickerKit.UI
{
    public sealed class IdleHudView : MonoBehaviour
    {
        [SerializeField]
        private IdleClickerManager manager;

        [Header("Text References")]
        [SerializeField]
        private TMP_Text coinsText;

        [SerializeField]
        private TMP_Text beadsText;

        [SerializeField]
        private TMP_Text lifetimeText;

        [SerializeField]
        private TMP_Text clickPowerText;

        [SerializeField]
        private TMP_Text passiveIncomeText;

        [SerializeField]
        private TMP_Text multiplierText;

        [SerializeField]
        private TMP_Text offlineRewardText;

        [SerializeField]
        private TMP_Text rewardBoostText;

        private void OnEnable()
        {
            if (manager != null)
            {
                manager.StateChanged += Refresh;
                manager.OfflineProgressApplied += OnOfflineProgressApplied;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (manager != null)
            {
                manager.StateChanged -= Refresh;
                manager.OfflineProgressApplied -= OnOfflineProgressApplied;
            }
        }

        public void Refresh()
        {
            if (manager == null || !manager.IsReady)
            {
                return;
            }

            if (coinsText != null)
            {
                coinsText.text = $"BioGel: {NumberFormatter.Compact(manager.Coins)}";
            }

            if (beadsText != null)
            {
                beadsText.text = $"Beads: {NumberFormatter.Compact(manager.Beads)}";
            }

            if (lifetimeText != null)
            {
                lifetimeText.text = $"Total BioGel: {NumberFormatter.Compact(manager.LifetimeCoins)}";
            }

            if (clickPowerText != null)
            {
                clickPowerText.text = $"Tap: +{NumberFormatter.Compact(manager.ClickPower)}";
            }

            if (passiveIncomeText != null)
            {
                passiveIncomeText.text = $"BioGel/s: +{NumberFormatter.Compact(manager.PassivePerSecond)}";
            }

            if (multiplierText != null)
            {
                multiplierText.text = $"x{manager.GlobalMultiplier:0.00}";
            }

            if (rewardBoostText != null)
            {
                rewardBoostText.text = manager.HasActiveRewardBoost
                    ? $"Reward boost: x{manager.ActiveRewardBoostMultiplier:0.00} ({manager.ActiveRewardBoostSecondsRemaining:0}s)"
                    : string.Empty;
            }
        }

        private void OnOfflineProgressApplied(OfflineProgressResult result)
        {
            if (offlineRewardText == null)
            {
                return;
            }

            if (result.earnedCoins <= 0d)
            {
                offlineRewardText.text = string.Empty;
                return;
            }

            offlineRewardText.text =
                $"+{NumberFormatter.Compact(result.earnedCoins)} from offline ({result.simulatedSeconds:0}s)";
        }
    }
}
