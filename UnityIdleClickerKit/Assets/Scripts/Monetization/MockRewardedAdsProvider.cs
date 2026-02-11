using System;
using System.Collections;
using UnityEngine;

namespace IdleClickerKit.Monetization
{
    public sealed class MockRewardedAdsProvider : RewardedAdsProviderBase
    {
        [SerializeField]
        [Min(0f)]
        private float simulatedWatchSeconds = 1.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float rewardSuccessChance = 1f;

        [SerializeField]
        private bool isReady = true;

        private bool isShowing = false;

        public override bool IsRewardedReady
        {
            get { return isReady && !isShowing; }
        }

        public override void ShowRewarded(Action<bool> onCompleted)
        {
            if (!IsRewardedReady)
            {
                onCompleted?.Invoke(false);
                return;
            }

            StartCoroutine(SimulateShow(onCompleted));
        }

        public void SetReady(bool value)
        {
            isReady = value;
        }

        private IEnumerator SimulateShow(Action<bool> onCompleted)
        {
            isShowing = true;
            if (simulatedWatchSeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(simulatedWatchSeconds);
            }

            isShowing = false;
            var rewarded = UnityEngine.Random.value <= rewardSuccessChance;
            onCompleted?.Invoke(rewarded);
        }
    }
}
