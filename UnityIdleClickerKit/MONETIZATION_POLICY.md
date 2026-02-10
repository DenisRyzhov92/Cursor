# Monetization Policy: Rewarded Ads + User-Initiated IAP

Этот проект настроен под строгую политику:

- Только rewarded-реклама.
- Показ рекламы запускается только по действию игрока (кнопка).
- Автоматический показ рекламы запрещен.
- Interstitial, App Open и forced ads не используются.
- Real-money покупки запускаются только по нажатию на конкретный продукт в магазине.

## Как это обеспечено в коде

- `RewardedOnlyMonetizationController` имеет флаг `requireUserInitiatedRequest = true`.
- Награда дается только через `RequestRewardedByUserAction()`.
- Прямого публичного API для авто-показа нет.
- `RealMoneyStoreController` имеет флаг `requireUserInitiatedPurchase = true`.
- Покупка выполняется через `RequestPurchaseByUserAction(productId)`.

## Требования к интеграции SDK

При подключении Unity Ads / AdMob / AppLovin:

1. Реализовать `RewardedAdsProviderBase`.
2. Подключать только rewarded unit id.
3. Не внедрять auto-trigger в геймплейные скрипты.

При подключении IAP SDK:

1. Реализовать `IapProviderBase`.
2. В текущем наборе можно использовать готовый `UnityIapProvider`.
3. Разрешать покупку только через UI-кнопки магазина.
4. Не вызывать покупку автоматически из игрового цикла.
