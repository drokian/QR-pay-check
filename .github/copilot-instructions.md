# GitHub Copilot — Proje Talimatları

Bu dosya, QR Pay Check projesinde Copilot'un her zaman uyması gereken kuralları tanımlar.

---

## PROJE SAHİPLİĞİ

- Bu proje kullanıcıya aittir. AI, projeyi kurmaya yardım etmiş olsa bile
  tüm kararlar kullanıcıya aittir.
- Kullanıcının doğrudan yaptığı değişiklikler, başka araçların (Copilot, 
  GitHub web editörü vb.) yaptığı değişiklikler projenin doğal parçasıdır.
- Beklenmedik commit veya değişiklik görüldüğünde kullanıcıya sor, 
  kendi bilgine göre yorumlama.

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

> **Bu adımlar her işlem bölümünde sırasıyla uygulanır. Atlanamaz.**

1. **Başlangıç:** `develop` branch'ını `origin/develop` ile hizala (`git checkout develop && git pull origin develop`).
2. **Branch oluştur:** `develop`'dan, DAL İSİMLENDİRME bölümündeki stratejiye uygun isimle yeni branch aç.
3. **Ortam kontrolü:** Port-sensitive komutlarda (`dotnet`, `npm`, `docker-compose` vb.) önce `cd <hedef_dizin>` çalıştır, sonra komutu çalıştır. Mevcut çalışma dizinine güvenme.
4. **Commit:** Birden fazla adım varsa her adımın sonunda yapılan işlemi açıklayıcı bir mesajla commitle (COMMIT KURALI bölümündeki formata uygun).
5. **Push izni:** ⛔ DUR. Bölüm tamamlandığında push yapmak için kullanıcıdan açık onay bekle. "push et", "push yapabilirsin" veya eşdeğer bir onay gelmeden `git push` komutu KESİNLİKLE çalıştırılmaz.
6. **PR izni:** ⛔ DUR. PR oluşturmak için kullanıcıdan açık onay bekle. Onay gelmeden `gh pr create` komutu KESİNLİKLE çalıştırılmaz. İzin gelince:
   - Türkçe, detaylı PR açıklaması yaz
   - Test plan adımlarını checkbox listesi olarak ekle
   - `--base develop` kullan
7. **Bilgilendir:** PR erişim linkini kullanıcıya ver.
8. **Review düzeltmeleri:** Review sonucunda gerekli düzeltmeleri yap, açıklayıcı commit mesajıyla commitle; ardından push için kullanıcıdan açık onay iste. 5. adımdaki kuralla aynı şekilde, açık izin gelmeden `git push` komutu KESİNLİKLE çalıştırılmaz.
9. **Tekrar review:** Ek review gelebilir — 8. adımı tekrarla.
10. **Merge sonrası:** ⛔ DUR. Kullanıcıdan "merge edildi" bilgisi VE "devam et" / "sonraki bölüme geç" onayı gelmeden bir sonraki bölüme KESİNLİKLE geçilmez. Bu onay gelmeden hiçbir yeni branch açılmaz, hiçbir kod yazılmaz.
11. **Döngü:** İzin gelince 1. adımdan tekrar başla.

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
STACK: .NET 10 + React/Vite + React Native + PostgreSQL + Keycloak + iyzico + SignalR + Turborepo
PATTERN: Clean Architecture (Wolverine CQRS, FluentValidation, Mapster)
REPO: https://github.com/drokian/QR-pay-check
DEFAULT_BRANCH: develop
```

### Dizin Yapısı
```
apps/api               → .NET 10 Web API (Clean Architecture)
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

---


## KULLANICI DEĞİŞİKLİKLERİ VE BAĞIMLILIK YÖNETİMİ

> **Bu kurallar mutlak olup atlanamaz.**

### Kullanıcı Değişiklikleri
- Kullanıcının doğrudan yaptığı değişiklikler **kesinlikle silinemez, ezilemez veya geri alınamaz.**
- `.gitignore`, config dosyaları dahil her dosyada kullanıcı değişikliği korunur.
- Bir dosyayı düzenlemeden önce kullanıcının o dosyada değişiklik yapıp yapmadığını kontrol et.
- git log'da beklenmedik commitler görürsen kendi yorumuna göre hareket etme — kullanıcıya sor.

### Versiyon & Bağımlılık Yönetimi
- SDK, runtime veya framework versiyonu (`.NET`, `Node.js`, `npm` vb.) **kullanıcı onayı olmadan değiştirilemez.**
- NuGet, npm veya diğer paket yöneticilerinde major veya minor versiyon yükseltmesi kullanıcı onayı gerektirir. Patch güncellemeler için de onay alınması önerilir.
- Versiyon yükseltmesi gerektiğinde şu formatta kullanıcıya sor:
  > "`X` şu an `A.B.C` versiyonunda. `A.B.D` versiyonuna yükseltmek istiyorum. Onaylıyor musunuz?"
- Onay gelmeden hiçbir `dotnet add package`, `npm install`, `dotnet tool update` veya benzeri komut çalıştırılmaz.

### Port Yönetimi
- Port-sensitive işlemlerde önce portun müsait olup olmadığını kontrol et:
  - WSL2: `ss -tlnp | grep :<port>`
  - Windows: `netstat -ano | findstr :<port>`
- Mevcut port durumunu kontrol etmeden yeni port ataması yapma.

--- 



