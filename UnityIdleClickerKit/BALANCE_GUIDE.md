# Balance Guide (Quick Tuning)

Эта шпаргалка помогает быстро подкрутить экономику перед релизом.

## Главные рычаги

Файл: `IdleClickerConfig.asset`

- `baseClickPower` — сколько даёт 1 тап в старте.
- `basePassivePerSecond` — стартовый idle-доход.
- `offlineIncomeEfficiency` — доля оффлайн-дохода (0..1).
- `offlineIncomeCapSeconds` — максимум секунд оффлайн-начисления.

## Формулы

- `TapIncome = (baseClickPower + sum(click bonuses)) * (1 + sum(multiplier bonuses))`
- `IdleIncomePerSec = (basePassivePerSecond + sum(passive bonuses)) * (1 + sum(multiplier bonuses))`
- `UpgradeCost(level) = baseCost * costGrowth^level`

## Практические значения для soft-launch

- `offlineIncomeEfficiency`: `0.5 - 0.8`
- `offlineIncomeCapSeconds`: `14400 - 28800` (4-8 часов)
- `costGrowth`: `1.14 - 1.30`
- `maxLevel` апгрейда: `15 - 50`

## Быстрый план балансировки

1. 10 минут активной игры: игрок должен открыть 3-4 апгрейда.
2. За 1 сессию (20-30 минут): минимум 1 ощутимый "рывок" прогресса.
3. Через 24 часа с оффлайном: игрок должен видеть заметный прирост, но не "слом" экономики.
