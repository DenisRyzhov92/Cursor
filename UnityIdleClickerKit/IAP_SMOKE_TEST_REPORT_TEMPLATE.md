# IAP Smoke Test Report Template

Используй этот шаблон после прогона:
- `IAP_SMOKE_TEST_CHECKLIST.md`

---

## Build / Environment

- Date:
- Tester:
- Build version:
- Branch / commit:
- Environment (internal / closed / production):
- Device model:
- OS version:
- Store region / currency:
- Network (Wi-Fi / LTE):

## Preconditions

- [ ] `UnityIapProvider` подключен (не mock)
- [ ] `RealMoneyStoreController -> Iap Provider` назначен
- [ ] Product IDs в игре = Product IDs в консоли
- [ ] Тестовый аккаунт покупателя авторизован

## Scenario Results

| Scenario | Result (PASS/FAIL/BLOCKED) | Notes | Evidence |
|---|---|---|---|
| S1. Store initialization |  |  |  |
| S2. Successful purchase |  |  |  |
| S3. User cancellation |  |  |  |
| S4. Invalid/unavailable product |  |  |  |
| S5. Double-tap protection |  |  |  |
| S6. Network recovery |  |  |  |
| S7. Restart after purchase |  |  |  |

## Defects Found

| ID | Severity | Scenario | Description | Repro Steps | Evidence | Status |
|---|---|---|---|---|---|---|
| BUG- | Critical/High/Medium/Low | Sx |  |  |  | Open |

## Product IDs Verified

| Product ID | Visible in store | Price loaded | Purchase success | Reward applied |
|---|---|---|---|---|
| iap_starter_supply |  |  |  |  |
| iap_terraform_booster |  |  |  |  |
| iap_colony_bundle |  |  |  |  |

## Final Decision

- Release readiness: **GO / NO-GO**
- Blocking issues:
  - 
- Follow-up actions:
  - 

## Attachments

- Video links:
- Screenshot links:
- Console logs / crash logs:
