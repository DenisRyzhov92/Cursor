# Space Farm Idle Clicker Kit

Готовый набор C#-скриптов для idle clicker с прокачкой в сеттинге космофермы, чтобы быстро собрать игру в Unity на Windows и довести до публикации в GetApps.

## Что уже готово

Папка: `UnityIdleClickerKit/Assets/Scripts`

- **Core**
  - `IdleClickerEngine` — экономика игры (клик, пассивный доход, апгрейды, цены, рост стоимости)
  - `IdleClickerManager` — MonoBehaviour-обертка для сцены, автосейв, оффлайн-доход, API для UI
  - `IdleSaveStorage` — сохранение/загрузка JSON в `Application.persistentDataPath`
  - `NumberFormatter` — компактный формат чисел (`12.5K`, `3.1M`)
- **Config**
  - `IdleClickerConfig` (ScriptableObject) — все стартовые параметры экономики
  - `UpgradeDefinition` — описание апгрейда (стоимость, бонусы, unlock-условие)
- **UI**
  - `IdleHudView` — обновляет HUD (BioGel, tap, BioGel/s, multiplier, offline reward)
  - `UpgradeButtonView` — логика кнопки апгрейда
  - `UpgradeListView` — автогенерация списка апгрейдов из конфига
  - `RewardedAdButtonView` — UI-кнопка для rewarded рекламы
  - `TapAreaView` — зона тапа, поддержка удержания
- **Monetization**
  - `RewardedOnlyMonetizationController` — rewarded-only логика наград (без авто-показа)
  - `RewardedAdsProviderBase` — абстракция провайдера rewarded рекламы
  - `MockRewardedAdsProvider` — тестовый провайдер для локальной проверки
- **Editor**
  - `Space Farm/Create Default Config` — меню для автосоздания дефолтного конфига с космофермерскими апгрейдами

## Быстрый старт в Unity (дома на Windows)

1. Создай новый проект в Unity (лучше 2022 LTS+, 2D Mobile).
2. Скопируй папку `UnityIdleClickerKit/Assets/Scripts` в свой `Assets/Scripts`.
3. Если TMP не инициализирован: `Window -> TextMeshPro -> Import TMP Essential Resources`.
4. В Unity нажми: **Space Farm -> Create Default Config**.
   - Создастся `Assets/Resources/IdleClickerConfig.asset`.
5. Создай пустой объект `GameManager`, добавь компонент `IdleClickerManager`.
6. Назначь в `IdleClickerManager` поле `Config` (созданный asset).
7. Сделай Canvas и добавь:
   - `IdleHudView` + ссылки на TMP_Text поля,
   - кнопку/панель тапа с `TapAreaView`,
   - `ScrollView` под апгрейды.
8. Создай prefab кнопки апгрейда с компонентом `UpgradeButtonView` и TMP-полями.
9. Добавь `UpgradeListView` на объект списка, укажи:
   - `Manager`,
   - `Upgrade Button Prefab`,
   - `Content Root`.
10. Для rewarded-only рекламы:
   - добавь объект `Monetization`,
   - повесь `MockRewardedAdsProvider` (для локального теста),
   - повесь `RewardedOnlyMonetizationController`,
   - привяжи `IdleClickerManager` и `RewardedAdsProvider`.
11. Создай кнопку "Watch ad", добавь `RewardedAdButtonView` и укажи контроллер монетизации.
12. Запусти сцену — клик, прокачка, сейвы, оффлайн-доход и rewarded-only поток работают.

## Концепция Space Farm

- Основной ресурс: **BioGel**.
- Тап = ручной сбор в куполе.
- Idle = автоматическая добыча дронами.
- Цель: пройти путь от мини-теплицы до терраформинга.

Дефолтная цепочка апгрейдов:
1. Manual Harvest Protocol
2. Micro Drone Swarm
3. Hydroponic Racks
4. Orbital Greenhouse
5. Solar Mirror Array
6. Terraforming AI

## Сохранения

- Файл сейва: `idle_clicker_save.json`
- Путь: `Application.persistentDataPath`
- Сохраняется:
  - BioGel (внутреннее поле `coins`),
  - lifetime BioGel,
  - уровни апгрейдов,
  - время последнего сейва (для оффлайн-начисления).

## Что быстро добавить перед релизом

1. **Rewarded ad only**: `x2 добыча на 30 минут` + мгновенная награда BioGel.
2. **IAP**:
   - стартовый бустер BioGel,
   - редкие кристаллы для косметики.
3. **Retention**:
   - ежедневный бонус,
   - 3-5 достижений,
   - миссии на сессию.

## Политика монетизации (жестко)

- Автоматической рекламы нет.
- Interstitial/App Open нет.
- Награда выдается только после добровольного нажатия игрока на кнопку "Watch ad".

## Чеклист публикации в GetApps

1. Android Build settings (IL2CPP, ARM64, min SDK по требованиям магазина).
2. Подпись release keystore.
3. Иконка, feature graphic, скриншоты.
4. Политика конфиденциальности (URL).
5. Локализованное описание (RU/EN минимум).
6. Включи аналитику и отслеживание retention/ARPDAU до масштабирования трафика.
