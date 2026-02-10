# Shop Setup (Beads + Real Money)

Этот файл описывает подключение двух магазинов:

1. **Beads Shop** — обмен BioGel на бусины.
2. **Real Money Shop** — полезные наборы за реальные деньги.

## 1) Beads Shop (BioGel -> Beads)

Скрипты:
- `BeadOfferButtonView`
- `BeadOfferListView`

Шаги:
1. Создай `ScrollView` с `Content`.
2. Создай prefab карточки оффера:
   - title (TMP),
   - description (TMP),
   - value (TMP),
   - cost (TMP),
   - state (TMP),
   - button.
3. Добавь на prefab `BeadOfferButtonView` и привяжи ссылки.
4. На `Content` добавь `BeadOfferListView`:
   - `Manager` = `IdleClickerManager`,
   - `Offer Button Prefab` = созданный prefab,
   - `Content Root` = Transform content.

## 2) Real Money Shop

Скрипты:
- `IapProviderBase` (абстракция)
- `MockIapProvider` (локальный тест)
- `RealMoneyStoreController`
- `RealMoneyProductButtonView`
- `RealMoneyProductListView`

Шаги:
1. Создай объект `IAP` и добавь:
   - `MockIapProvider`,
   - `RealMoneyStoreController`.
2. В `RealMoneyStoreController` привяжи:
   - `Manager` = `IdleClickerManager`,
   - `Iap Provider` = `MockIapProvider`.
3. Создай prefab карточки IAP-продукта:
   - title (TMP),
   - description (TMP),
   - value/reward (TMP),
   - price (TMP),
   - state (TMP),
   - purchase button.
4. Добавь `RealMoneyProductButtonView` на карточку и привяжи ссылки.
5. Добавь `RealMoneyProductListView` на content списка:
   - `Manager`,
   - `Store Controller`,
   - `Product Button Prefab`,
   - `Content Root`.

## Дефолтные продукты из конфига

- `iap_starter_supply` — быстрый стартовый набор.
- `iap_terraform_booster` — бустер с временным множителем.
- `iap_colony_bundle` — крупный полезный набор для mid-game.

## Политика запуска покупок

- Только по пользовательскому действию.
- Авто-покупок нет.
- Реальная интеграция должна использовать `IapProviderBase`.
