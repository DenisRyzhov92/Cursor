# Shop Setup (Progress Boosts + Real Money)

Этот файл описывает подключение двух магазинов:

1. **Progress Boost Shop** — ускорители прогресса за BioGel.
2. **Real Money Shop** — полезные наборы за реальные деньги.

## 1) Progress Boost Shop (BioGel -> temporary boosts)

Скрипты:
- `ProgressBoostOfferButtonView`
- `ProgressBoostOfferListView`

Шаги:
1. Создай `ScrollView` с `Content`.
2. Создай prefab карточки оффера:
   - title (TMP),
   - description (TMP),
   - value (TMP),
   - cost (TMP),
   - state (TMP),
   - button.
3. Добавь на prefab `ProgressBoostOfferButtonView` и привяжи ссылки.
4. На `Content` добавь `ProgressBoostOfferListView`:
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

Boost-офферы (кривая soft-launch):
- Early:
  - `boost_ion_pulse` — x1.35 на 120с.
  - `boost_solar_focus` — x1.5 на 180с + небольшой instant drop.
  - `boost_drone_overclock` — x1.75 на 210с.
- Mid:
  - `boost_orbital_sync` — x2.0 на 240с.
  - `boost_bioreactor_surge` — x2.35 на 300с.
- Late:
  - `boost_plasma_wave` — x2.8 на 360с.
  - `boost_terraform_rush` — x3.3 на 420с.

IAP utility packs:
- `iap_starter_supply` — быстрый стартовый набор.
- `iap_terraform_booster` — бустер с временным множителем.
- `iap_colony_bundle` — крупный полезный набор для mid-game.

## Политика запуска покупок

- Только по пользовательскому действию.
- Авто-покупок нет.
- Реальная интеграция должна использовать `IapProviderBase`.
