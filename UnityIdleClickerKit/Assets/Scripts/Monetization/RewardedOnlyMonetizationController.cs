using System;
using IdleClickerKit.Core;
using UnityEngine;
using UnityEngine.Events;

namespace IdleClickerKit.Monetization
{
    public sealed class RewardedOnlyMonetizationController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private IdleClickerManager manager;

        [SerializeField]
        private RewardedAdsProviderBase rewardedAdsProvider;

        [Header("Policy")]
        [SerializeField]
        [Tooltip("Never show ad automatically. Ads can be started only by user action.")]
        private bool requireUserInitiatedRequest = true;

        [Header("Reward")]
        [SerializeField]
        [Tooltip("Reward BioGel equal to this amount of passive income seconds.")]
        [Min(0f)]
        private float passiveSecondsAsCoins = 300f;

        [SerializeField]
        [Min(0f)]
        private float flatCoinsReward = 0f;

        [SerializeField]
        private bool grantTemporaryBoost = true;

        [SerializeField]
        [Min(1f)]
        private float temporaryBoostMultiplier = 2f;

        [SerializeField]
        [Min(0f)]
        private float temporaryBoostDurationSeconds = 1800f;

        [SerializeField]
        private bool replaceCurrentBoostIfActive = true;

        [Header("Cooldown")]
        [SerializeField]
        [Min(0f)]
        private float cooldownSeconds = 90f;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onRewardGranted;

        [SerializeField]
        private UnityEvent onRewardRejected;

        private float cooldownRemaining = 0f;
        private bool isShowingAd = false;

        public event Action StateChanged;

        public bool IsShowingAd
        {
            get { return isShowingAd; }
        }

        public float CooldownRemaining
        {
            get { return Mathf.Max(0f, cooldownRemaining); }
        }

        public bool CanShowRewarded
        {
            get
            {
                if (isShowingAd || CooldownRemaining > 0f)
                {
                    return false;
                }

                if (manager == null || !manager.IsReady || rewardedAdsProvider == null)
                {
                    return false;
                }

                if (!rewardedAdsProvider.IsRewardedReady)
                {
                    return false;
                }

                if (!replaceCurrentBoostIfActive && manager.HasActiveRewardBoost)
                {
                    return false;
                }

                return true;
            }
        }

        private void Update()
        {
            if (cooldownRemaining <= 0f)
            {
                return;
            }

            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.unscaledDeltaTime);
            StateChanged?.Invoke();
        }

        public void RequestRewardedByUserAction()
        {
            TryShowRewarded(true);
        }

        private void TryShowRewarded(bool isUserInitiated)
        {
            if (requireUserInitiatedRequest && !isUserInitiated)
            {
                return;
            }

            if (!CanShowRewarded)
            {
                onRewardRejected?.Invoke();
                StateChanged?.Invoke();
                return;
            }

            isShowingAd = true;
            StateChanged?.Invoke();
            rewardedAdsProvider.ShowRewarded(HandleRewardedCompleted);
        }

        public string GetStatusText()
        {
            if (manager == null || !manager.IsReady)
            {
                return "Game manager missing";
            }

            if (rewardedAdsProvider == null)
            {
                return "Ads provider missing";
            }

            if (isShowingAd)
            {
                return "Ad is running...";
            }

            if (!rewardedAdsProvider.IsRewardedReady)
            {
                return "Rewarded ad is not ready";
            }

            if (CooldownRemaining > 0f)
            {
                return $"Available in {CooldownRemaining:0}s";
            }

            if (!replaceCurrentBoostIfActive && manager.HasActiveRewardBoost)
            {
                return "Boost already active";
            }

            return "Ready";
        }

        private void HandleRewardedCompleted(bool rewarded)
        {
            isShowingAd = false;

            if (!rewarded)
            {
                onRewardRejected?.Invoke();
                StateChanged?.Invoke();
                return;
            }

            ApplyReward();
            cooldownRemaining = cooldownSeconds;
            onRewardGranted?.Invoke();
            StateChanged?.Invoke();
        }

        private void ApplyReward()
        {
            if (manager == null || !manager.IsReady)
            {
                return;
            }

            var coins = manager.PassivePerSecond * passiveSecondsAsCoins + flatCoinsReward;
            if (coins > 0d)
            {
                manager.GrantRewardCoins(coins);
            }

            if (grantTemporaryBoost && temporaryBoostMultiplier > 1f && temporaryBoostDurationSeconds > 0f)
            {
                if (replaceCurrentBoostIfActive || !manager.HasActiveRewardBoost)
                {
                    manager.ActivateRewardBoost(temporaryBoostDurationSeconds, temporaryBoostMultiplier);
                }
            }
        }
    }
}
