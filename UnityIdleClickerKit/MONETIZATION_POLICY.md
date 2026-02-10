# Monetization Policy: Rewarded Only

Этот проект настроен под строгую политику:

- Только rewarded-реклама.
- Показ рекламы запускается только по действию игрока (кнопка).
- Автоматический показ рекламы запрещен.
- Interstitial, App Open и forced ads не используются.

## Как это обеспечено в коде

- `RewardedOnlyMonetizationController` имеет флаг `requireUserInitiatedRequest = true`.
- Награда дается только через `RequestRewardedByUserAction()`.
- Прямого публичного API для авто-показа нет.

## Требования к интеграции SDK

При подключении Unity Ads / AdMob / AppLovin:

1. Реализовать `RewardedAdsProviderBase`.
2. Подключать только rewarded unit id.
3. Не внедрять auto-trigger в геймплейные скрипты.
