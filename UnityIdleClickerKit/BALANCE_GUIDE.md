# Balance Guide (Quick Tuning)

Эта шпаргалка помогает быстро подкрутить экономику космофермы перед релизом.

## Главные рычаги

Файл: `IdleClickerConfig.asset`

- `baseClickPower` — сколько BioGel даёт 1 тап в старте.
- `basePassivePerSecond` — стартовый idle-доход.
- `offlineIncomeEfficiency` — доля оффлайн-дохода (0..1).
- `offlineIncomeCapSeconds` — максимум секунд оффлайн-начисления.

## Формулы

- `TapIncome = (baseClickPower + sum(click bonuses)) * (1 + sum(multiplier bonuses))`
- `IdleIncomePerSec = (basePassivePerSecond + sum(passive bonuses)) * (1 + sum(multiplier bonuses))`
- `UpgradeCost(level) = baseCost * costGrowth^level`

## Дефолтная кривая boost-офферов (7 штук)

Early:
1. `boost_ion_pulse` — cost `120`, x`1.35`, `120s`
2. `boost_solar_focus` — cost `350`, x`1.50`, `180s`, instant `60`
3. `boost_drone_overclock` — cost `900`, x`1.75`, `210s`, instant `180`

Mid:
4. `boost_orbital_sync` — cost `2200`, x`2.00`, `240s`, instant `420`
5. `boost_bioreactor_surge` — cost `5600`, x`2.35`, `300s`, instant `1300`

Late:
6. `boost_plasma_wave` — cost `14500`, x`2.80`, `360s`, instant `3600`
7. `boost_terraform_rush` — cost `36000`, x`3.30`, `420s`, instant `9500`

## Практические значения для soft-launch

- `offlineIncomeEfficiency`: `0.5 - 0.8`
- `offlineIncomeCapSeconds`: `14400 - 28800` (4-8 часов)
- `costGrowth`: `1.14 - 1.30`
- `maxLevel` апгрейда: `15 - 50`

## Быстрый план балансировки

1. 10 минут активной игры: игрок должен открыть 3-4 апгрейда.
2. За 1 сессию (20-30 минут): минимум 1 ощутимый "рывок" прогресса.
3. Через 24 часа с оффлайном: игрок должен видеть заметный прирост, но не "слом" экономики.
