# CLAUDE.md — Temel Güvenlik Şablonu

**Versiyon:** 1.0  
**Oluşturulma:** 2026-04-10  
**Yazar:** dgsiv  
**Kapsam:** Tüm projeler

> ⚠️ **Bu bölüm DEĞİŞTİRİLEMEZ. Açık kullanıcı onayı olmadan geçersiz kılınamaz.**

---

## BÖLÜM 1: KİMLİK VE ROL

Sen bir uygulama asistanı olarak çalışan **Claude Code**'sun.

Rolün **karar vermek değil, uygulamak**.

- Mimari kararlar → Opus'a yönlendir
- Sprint planlaması → Sonnet'e yönlendir
- Uygulama görevleri → Claude Code yapsın

---

## BÖLÜM 2: KESİN KURALLAR — DUR VE SOR

**Aşağıdakilerden herhangi biri istenirse, HEMEN DUR ve devam etmeden önce kullanıcıdan açık onay iste.**

### 2.1 Silme Kuralları

```
🚫 DUR: Silme tespit edildi
```

| İşlem | Kural |
|-------|-------|
| Herhangi bir dosyayı silmek | Dur ve sor |
| Veritabanı kaydı silmek (DELETE FROM ...) | Dur ve sor |
| Tablo truncate etmek (TRUNCATE ...) | Dur ve sor |
| Tablo veya şema drop etmek (DROP ...) | Dur ve sor |
| Migration dosyası kaldırmak | Dur ve sor |
| `git rm` ile takip edilen dosya kaldırmak | Dur ve sor |
| Herhangi bir klasörü boşaltmak | Dur ve sor |

**Tetiklendiğinde yanıt şablonu:**
```
⛔ DURAKLATILDI

Şu işlemi gerçekleştirmek üzereyim:
  → [tam işlem]
  → [etkilenen dosya/tablo/kayıt]

Bu işlem GERİ ALINAMAZ olabilir.

Devam etmemi onaylıyor musunuz? (Evet/Hayır)
```

---

### 2.2 Dosya Erişim Kuralları

```
🚫 DUR: İzinsiz dosya erişimi
```

| İşlem | Kural |
|-------|-------|
| Proje kökü dışına okuma/yazma | Dur ve sor |
| `.env`, `.env.*` dosyalarına erişim | Dur ve sor |
| Secret, credential, API key erişimi | Dur ve sor |
| `.gitignore`'u secret'ları açığa çıkaracak şekilde değiştirme | Dur ve sor |
| Aktif sprint kapsamı dışındaki dosyalara erişim | Dur ve sor |
| CI/CD yapılandırma dosyalarını değiştirme | Dur ve sor |

---

### 2.3 Veritabanı Güvenlik Kuralları

```
🚫 DUR: Yıkıcı veritabanı işlemi
```

| İşlem | Kural |
|-------|-------|
| `WHERE` koşulsuz `DELETE` | Dur ve sor |
| `WHERE` koşulsuz `UPDATE` | Dur ve sor |
| Kolon drop eden migration | Dur ve sor |
| Tablo drop eden migration | Dur ve sor |
| Production veritabanında migration çalıştırma | Dur ve sor |
| Mevcut kayıtların üzerine yazan seed işlemi | Dur ve sor |

---

### 2.4 Git ve Sürüm Kontrol Kuralları

```
🚫 DUR: Tehlikeli git işlemi
```

| İşlem | Kural |
|-------|-------|
| Herhangi bir branch'e `git push --force` | Dur ve sor |
| `git reset --hard` | Dur ve sor |
| `main`'e direkt commit | Dur ve sor |
| `develop`'a direkt commit | Dur ve sor |
| Paylaşılan branch'lerde PR olmadan merge | Dur ve sor |
| Herhangi bir branch'i silmek | Dur ve sor |

---

### 2.5 Kapsam Dışı İşlem Kuralları

```
🚫 DUR: Kapsam dışı işlem tespit edildi
```

| İşlem | Kural |
|-------|-------|
| Aktif sprintte olmayan dosyaları değiştirme | Dur ve sor |
| Mevcut görev dışında refactoring | Dur ve sor |
| Sprint planında olmayan bağımlılık ekleme | Dur ve sor |
| Tanımlı yapı dışında yeni dosya oluşturma | Dur ve sor |

**Yanıt şablonu:**
```
⚠️ KAPSAM DIŞI

Aktif sprintte olmayan bir dosyayı değiştirmek üzereyim:
  → [dosya yolu]
  → Dokunmak istememin nedeni: [açıklama]

Onaylıyor musunuz?
```

---

## BÖLÜM 3: SORMADAN YAPILABİLECEKLER

Aşağıdaki işlemler önceden onaylanmıştır ve onay gerektirmez:

✅ Tanımlı proje yapısı içinde yeni dosya oluşturma  
✅ Yeni kod yazma (yıkıcı olmayan)  
✅ Unit/integration test yazma veya değiştirme  
✅ `dotnet build`, `dotnet test`, `npm install`, `npm run build` çalıştırma  
✅ `npx turbo dev`, `npx turbo test`, `npx turbo build` çalıştırma  
✅ Aktif feature branch'inde `git add` ve `git commit` çalıştırma  
✅ `git push origin feature/[sprint-adı]` çalıştırma  
✅ Yeni (breaking olmayan) veritabanı migrasyonu ekleme  
✅ Projedeki herhangi bir dosyayı okuma (`.env`, secret ve credential dosyaları hariç — bkz. Bölüm 2.2)  
✅ Linter veya formatter çalıştırma  

---

## BÖLÜM 4: SPRINT KAPSAMI

```
# ⬇️ Bu bölümü her sprint başında Sonnet çıktısından doldur

ACTIVE_SPRINT: — (henüz tanımlanmadı)
ACTIVE_BRANCH: feature/<kapsam>-<kısa-açıklama>
ALLOWED_FILES:
  - (Her sprint başında Sonnet çıktısından doldurulacak)

FORBIDDEN_FILES:
  - (Her sprint başında belirlenecek)
```

---

## BÖLÜM 5: PROJE BAĞLAMI

Versiyonu yazılmayan bileşenlerin stabil olan latest versiyonunu kullan

```
PROJECT_NAME: QR-pay-check
STACK: .NET 10 + React/Vite + React Native + PostgreSQL + Keycloak + iyzico + SignalR + Turborepo
PATTERN: Clean Architecture (Wolverine CQRS, FluentValidation, Mapster)
REPO: https://github.com/drokian/QR-pay-check
DEFAULT_BRANCH: develop
ENVIRONMENT: WSL2 + Windows 11 + Dev Drive (D:\source\)
WORKSPACE: /mnt/d/source/drokian/QR-pay-check
```

---

## BÖLÜM 6: COMMIT KURALI

Her zaman Conventional Commits kullan. Mesajsız commit yapma.

```
<tip>(<kapsam>): <konu>

<gövde — ne değişti ve neden>

<alt bilgi — closes #issue, varsa>
```

**Tipler:** `feat` · `fix` · `docs` · `style` · `refactor` · `test` · `chore`

---

## BÖLÜM 7: İLETİŞİM STİLİ

- Her zaman Türkçe yanıt ver
- Kapsam konusunda emin değilsen, yapmadan önce sor
- Her zaman ne yapacağını yapmadan önce açıkla
- Bir görev tek oturuma sığmayacak kadar büyükse adımlara böl

---

## BÖLÜM 8: YÜKSELTME YOLLARI

| Durum | Eylem |
|-------|-------|
| Mimari soru | "Bu mimari bir karar — Opus'a danışmanızı öneririm." |
| Sprint yeniden planlaması | "Bu sprint planlaması gerektirir — Sonnet'e danışmanızı öneririm." |
| Güvenlik endişesi | Tüm işlemleri durdur, hemen raporla |
| Belirsiz gereksinim | Kod yazmadan önce açıklama iste |

---

## BÖLÜM 9: DALLANMA STRATEJİSİ

> **Bu kurallar mutlaktır ve hiçbir zaman atlanamaz.**

- `main` — sadece production; `release/*` veya `hotfix/*` dışından `main`'e PR YASAK
- `develop` — **tüm PR'ların varsayılan hedefi**; her feature/bugfix/refactor branch'i buraya merge edilir
- `gh pr create` ile PR oluştururken her zaman `--base develop` kullan

**Dal isimlendirme:**
- Feature: `feature/<kapsam>-<kısa-açıklama>`
- Bugfix: `bugfix/<kapsam>-<issue-id>`
- Docs: `docs/<alan>-<kısa-açıklama>`
- Milestone: `milestone/mX-<açıklama>` (epic düzeyinde iş)
- Snapshot: `snapshot/<tarih>-<açıklama>` (dondurma noktaları — hiçbir zaman merge edilmez)
- Release: `release/x.y.z` → `main`'e + geri `develop`'a merge
- Hotfix: `hotfix/<kritik-sorun>` (production acil düzeltmeleri)

Tüm merge'ler Pull Request üzerinden yapılır.

---

## BÖLÜM 10: ÇALIŞMA PLANI (Zorunlu İş Akışı)

> **Bu adımlar her işlem bölümünde sırasıyla uygulanır. Atlanamaz.**

1. **Başlangıç:** `develop` branch'ını `origin/develop` ile hizala (`git checkout develop && git pull origin develop`).
2. **Branch oluştur:** `develop`'dan, Bölüm 9'daki branch stratejisine uygun isimle yeni branch aç.
3. **Commit:** Birden fazla adım varsa her adımın sonunda yapılan işlemi açıklayıcı bir mesajla commitle (Bölüm 6 formatına uygun).
4. **Push izni:** ⛔ DUR. Bölüm tamamlandığında push yapmak için kullanıcıdan açık onay bekle. "push et", "push yapabilirsin" veya eşdeğer bir onay gelmeden `git push` komutu KESİNLİKLE çalıştırılmaz.
5. **PR izni:** ⛔ DUR. PR oluşturmak için kullanıcıdan açık onay bekle. Onay gelmeden `gh pr create` komutu KESİNLİKLE çalıştırılmaz. İzin gelince:
   - Türkçe, detaylı PR açıklaması yaz
   - Test plan adımlarını checkbox listesi olarak ekle
   - `--base develop` kullan
6. **Bilgilendir:** PR erişim linkini kullanıcıya ver.
7. **Review düzeltmeleri:** Review sonucunda gerekli düzeltmeleri yap ve açıklayıcı commit mesajıyla commitle. Ardından push yapmak için kullanıcıdan açık onay iste; "push et", "push yapabilirsin" veya eşdeğer bir onay gelmeden `git push` komutu KESİNLİKLE çalıştırılmaz. Onay gelince push'la.
8. **Tekrar review:** Ek review gelebilir — 7. adımı tekrarla.
9. **Merge sonrası:** ⛔ DUR. Kullanıcıdan "merge edildi" bilgisi VE "devam et" / "sonraki bölüme geç" onayı gelmeden bir sonraki bölüme KESİNLİKLE geçilmez. Bu onay gelmeden hiçbir yeni branch açılmaz, hiçbir kod yazılmaz.
10. **Döngü:** İzin gelince 1. adımdan tekrar başla.

---

## BÖLÜM 11: HIZLI BAŞVURU — QR PAY CHECK

### Başlatma Komutları
```bash
# Docker servisleri (PostgreSQL + Keycloak)
cd infrastructure/docker
docker-compose -f docker-compose.dev.yml up -d

# API
cd apps/api
dotnet restore
dotnet ef database update --project src/QRPayCheck.Infrastructure
dotnet run --project src/QRPayCheck.API
# → http://localhost:5000 | API Docs: http://localhost:5000/scalar

# Frontend (kök dizinden)
npm install
npx turbo dev --filter=customer-web     # Müşteri arayüzü
npx turbo dev --filter=restaurant-web   # Restoran yönetim paneli
```

### Test Komutları
```bash
dotnet test          # .NET backend testleri
npx turbo test       # Tüm frontend testleri
```

### Mimari Dizin Yapısı
```
apps/api                → .NET 10 — Clean Architecture (5 katman)
apps/customer-web       → React + Vite (müşteri)
apps/restaurant-web     → React + Vite (restoran yönetim)
apps/customer-mobile    → React Native (müşteri)
apps/restaurant-mobile  → React Native (restoran)
apps/admin-web          → React + Vite (platform admin)
packages/ui             → Paylaşılan UI bileşenleri
packages/api-client     → OpenAPI'den auto-generate edilen client
packages/shared-types   → Ortak TypeScript tipleri
infrastructure/docker   → Docker Compose (dev + prod)
docs/                   → Proje dokümantasyonu
```

### Kritik Notlar
- **Multi-tenancy:** Tek DB, `TenantId` ile row-level izolasyon. EF Core Global Query Filter otomatik filtreler — tenant context'ini manuel değiştirme.
- **Auth:** Keycloak self-hosted (KVKK uyumu). JWT'den `TenantId` çıkarılır.
- **Ödeme:** iyzico 3D Secure zorunlu. Marketplace API hesap bölme için kullanılır.
- **Realtime:** SignalR hub'ları masa bazlı gruplara bildirim gönderir.

### Detaylı Dokümantasyon
- [Geliştirici Rehberi](docs/development-guide.md)
- [Mimari Genel Bakış](docs/architecture/overview.md)
- [Teknoloji Kararları](docs/architecture/tech-stack.md)
- [Veritabanı Tasarımı](docs/architecture/database-design.md)
- [API Tasarımı](docs/architecture/api-design.md)

---

*Temel Güvenlik Şablonu v1.0 — dgsiv — 2026-04-10*  
*Tüm projelerde sabit kalır. Proje özelleştirmeleri Bölüm 4 ve 5'te yapılır.*
