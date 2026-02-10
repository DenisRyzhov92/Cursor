# Unity IAP Setup (Production)

Этот гайд переводит магазин с `MockIapProvider` на реальную оплату через Unity Purchasing.

## 1) Установи Unity Purchasing

В Unity:
1. `Window -> Package Manager`
2. Найди пакет `In-App Purchasing` (Unity Purchasing)
3. Установи пакет

После установки в проекте должен быть доступен `UNITY_PURCHASING`.

## 2) Подготовь product IDs

Источник ID в проекте:
- `IdleClickerConfig.realMoneyProducts[].productId`

Текущие дефолтные ID:
- `iap_starter_supply`
- `iap_terraform_booster`
- `iap_colony_bundle`

Создай такие же ID в магазине (Google Play Console / App Store Connect).

## 3) Замени провайдер

В объекте `IAP`:
- убери `MockIapProvider`,
- добавь `UnityIapProvider`,
- оставь `RealMoneyStoreController`.

В `RealMoneyStoreController` поле `Iap Provider` укажи `UnityIapProvider`.

## 4) Настрой UnityIapProvider

Рекомендуемые настройки:
- `Auto Initialize On Start` = ON
- `Include Products From Config` = ON
- `Resources Config Name` = `IdleClickerConfig`
- `Default Product Type` = `Consumable`

Если нужно, добавь дополнительные IDs в `Additional Product Ids`.

## 5) Проверка на устройстве

1. Собери release-билд с тем же application id, который в консоли магазина.
2. Используй внутреннее тестирование (internal testing / closed track).
3. Проверь:
   - корректный вывод цены в UI,
   - успешную покупку,
   - выдачу наград в `RealMoneyStoreController -> IdleClickerManager`.

## 6) Важные ограничения

- Покупки запускаются только по действию пользователя (кнопка товара).
- Автопокупки из игрового цикла запрещены.
- В проекте только rewarded ads + user-initiated IAP.

## Troubleshooting

- Нет цен в UI:
  - проверь, что магазин инициализировался,
  - совпадают ли `productId` в игре и консоли.
- Покупка не выдает награду:
  - проверь `productId` в `IdleClickerConfig`,
  - проверь связь `RealMoneyStoreController` с `IdleClickerManager`.
