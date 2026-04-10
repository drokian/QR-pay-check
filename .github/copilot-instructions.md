# GitHub Copilot — Proje Talimatları

Bu dosya, QR Pay Check projesinde Copilot'un her zaman uyması gereken kuralları tanımlar.

---

## ROL

Rolün **karar vermek değil, uygulamak**. Mimari sorular için "Bu mimari bir karar" de, güvenlik sorunlarını anında raporla, belirsiz gereksinimlerde kod yazmadan önce açıklama iste. Her zaman Türkçe yanıt ver.

---

## KESİN KURALLAR — HEMEN DUR VE SOR

Aşağıdaki işlemleri kullanıcıdan açık onay almadan **asla** yapma:

### Silme
- Herhangi bir dosyayı silmek
- `DELETE FROM`, `TRUNCATE`, `DROP TABLE/SCHEMA`
- Migration dosyası kaldırmak
- `git rm` ile takip edilen dosya kaldırmak

### Dosya Erişimi
- `.env`, `.env.*`, secret, credential, API key dosyalarına erişim
- `.gitignore`'u secret'ları açığa çıkaracak şekilde değiştirme

### Veritabanı
- `WHERE` koşulsuz `DELETE` veya `UPDATE`
- Kolon/tablo drop eden migration
- Production DB'de migration çalıştırma

### Git
- `git push --force` (herhangi bir branch)
- `git reset --hard`
- `main` veya `develop`'a direkt commit
- Herhangi bir branch'i silmek

**Tetiklendiğinde şu şablonu kullan:**
```
⛔ DURAKLATILDI
→ [yapılmak istenen işlem]
→ [etkilenen kaynak]
Bu işlem geri alınamaz olabilir. Devam etmemi onaylıyor musunuz?
```

---

## SORMADAN YAPILABİLECEKLER

- Tanımlı proje yapısı içinde yeni dosya oluşturma
- Yeni kod yazma (yıkıcı olmayan)
- Test yazma/değiştirme
- `dotnet build/test`, `npm install/build`, `npx turbo dev/test/build`
- Aktif feature branch'inde `git add` ve `git commit`
- Yeni (breaking olmayan) DB migrasyonu ekleme
- Projedeki dosyaları okuma (`.env` ve credential dosyaları hariç)
- Linter/formatter çalıştırma

---

## ZORUNLU ÇALIŞMA PLANI

Her geliştirme bölümünde bu adımları sırayla uygula, atlama:

1. `git checkout develop && git pull origin develop`
2. `develop`'dan uygun isimle yeni branch aç (bkz. Dal İsimlendirme)
3. Her adım sonunda Conventional Commits formatıyla commitle
4. Bölüm bitince **push için kullanıcıdan izin iste**, izin gelince push'la
5. **PR için kullanıcıdan izin iste**, izin gelince: Türkçe detaylı açıklama + test checklist + `--base develop` ile PR oluştur
6. PR linkini kullanıcıya ver
7. Review gelince gerekli düzeltmeleri yap, commitle; **push için kullanıcıdan izin iste**, izin gelince push'la
8. Ek review gelebilir — 7. adımı tekrarla
9. "Merge edildi" bilgisi gelince sonraki bölüme geçmek için izin iste
10. İzin gelince 1. adımdan başla

---

## DAL İSİMLENDİRME

| Tip | Format | Örnek |
|-----|--------|-------|
| Feature | `feature/<kapsam>-<kısa-açıklama>` | `feature/menu-crud` |
| Bugfix | `bugfix/<kapsam>-<issue-id>` | `bugfix/order-total-123` |
| Docs | `docs/<alan>-<kısa-açıklama>` | `docs/architecture-update` |
| Milestone | `milestone/mX-<açıklama>` | `milestone/m1-infra-setup` |
| Snapshot | `snapshot/<tarih>-<açıklama>` | `snapshot/2026-04-11-mvp1` |
| Release | `release/x.y.z` | `release/1.0.0` |
| Hotfix | `hotfix/<kritik-sorun>` | `hotfix/payment-callback` |

- `main`'e PR: sadece `release/*` veya `hotfix/*`'dan
- Tüm diğer PR'lar → `--base develop`
- Tüm merge'ler Pull Request üzerinden

---

## COMMIT KURALI

```
<tip>(<kapsam>): <konu>

<ne değişti ve neden>

<closes #issue — varsa>
```

**Tipler:** `feat` · `fix` · `docs` · `style` · `refactor` · `test` · `chore`

---

## PROJE BAĞLAMI

```
PROJECT: QR Pay Check
STACK: .NET Core + React/Vite + React Native + PostgreSQL + Keycloak + iyzico + SignalR + Turborepo
PATTERN: Clean Architecture (CQRS, FluentValidation, Mapster)
REPO: https://github.com/drokian/QR-pay-check
DEFAULT_BRANCH: develop
```

### Dizin Yapısı
```
apps/api               → .NET Core Web API (Clean Architecture)
apps/customer-web      → React + Vite (müşteri)
apps/restaurant-web    → React + Vite (restoran yönetim)
apps/customer-mobile   → React Native (müşteri)
apps/restaurant-mobile → React Native (restoran)
apps/admin-web         → React + Vite (platform admin)
packages/ui            → Paylaşılan UI bileşenleri
packages/api-client    → OpenAPI'den auto-generate client
packages/shared-types  → Ortak TypeScript tipleri
infrastructure/docker  → Docker Compose
docs/                  → Proje dokümantasyonu
development/           → Dahili notlar (.gitignore'da)
```

### Kritik Notlar
- **Multi-tenancy:** Tek DB, `TenantId` ile row-level izolasyon. EF Core Global Query Filter otomatik filtreler.
- **Auth:** Keycloak self-hosted. JWT'den `TenantId` çıkarılır.
- **Ödeme:** iyzico 3D Secure zorunlu.
- **Realtime:** SignalR, masa bazlı gruplara bildirim gönderir.
