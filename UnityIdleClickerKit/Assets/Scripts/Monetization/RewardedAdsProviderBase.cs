using System;
using UnityEngine;

namespace IdleClickerKit.Monetization
{
    public abstract class RewardedAdsProviderBase : MonoBehaviour
    {
        public abstract bool IsRewardedReady { get; }
        public abstract void ShowRewarded(Action<bool> onCompleted);
    }
}
