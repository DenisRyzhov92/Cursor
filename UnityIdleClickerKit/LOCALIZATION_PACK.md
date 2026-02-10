# Space Farm Localization Pack (RU + EN)

Готовый пакет текстов для:
- названия игры,
- карточки приложения в GetApps,
- in-game локализации UI (через CSV).

## Рекомендуемый нейминг

- **Основное название (RU):** Космоферма: BioGel Империя
- **Основное название (EN):** Space Farm: BioGel Empire

Альтернативы:
1. Космоферма: Терраформер / Space Farm: Terraformer
2. BioDome Tycoon / BioDome Tycoon
3. Орбитальная Ферма / Orbital Farm Idle

## Что лежит в этом пакете

- `StoreListing/GetApps/ru-RU.md` — готовые RU тексты для публикации.
- `StoreListing/GetApps/en-US.md` — готовые EN тексты для публикации.
- `Localization/in_game_strings.csv` — ключи и строки для UI + апгрейдов.
- `SHOP_SETUP.md` — подключение магазинов (Beads + IAP).

## Как использовать быстро

1. Выбери одно финальное название (лучше из рекомендованного).
2. Скопируй short/long description из RU и EN файлов в консоль GetApps.
3. Импортируй `in_game_strings.csv` в систему локализации Unity (или подключи вручную).
4. Проверь единообразие терминов:
   - BioGel (основной ресурс),
   - Drone/Дрон,
   - Terraforming/Терраформинг.

## Правило монетизации в текстах

Во всех описаниях используется only rewarded подход:
- без автопоказа рекламы,
- без interstitial/app-open,
- награда только по кнопке игрока.
