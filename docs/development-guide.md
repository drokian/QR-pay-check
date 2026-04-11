# Geliştirici Rehberi

## Ön Gereksinimler

| Araç | Versiyon | Açıklama |
|------|----------|----------|
| .NET SDK | 10.0+ | Backend API |
| Node.js | 20 LTS+ | Frontend build & tooling |
| Docker Desktop | latest | PostgreSQL, Keycloak, API container |
| Git | latest | Versiyon kontrol |
| VS Code / Rider | latest | IDE |

## Kurulum

### 1. Repo Klonlama
```bash
git clone https://github.com/drokian/QR-pay-check.git
cd QR-pay-check
```

### 2. Docker Servisleri Başlatma
```bash
cd infrastructure/docker
docker-compose -f docker-compose.dev.yml up -d
```

Bu komut şunları ayağa kaldırır:
- PostgreSQL (port 5434)
- Keycloak (port 8084)

### 3. API Başlatma
```bash
cd apps/api
dotnet restore
dotnet ef database update --project src/QRPayCheck.Infrastructure
dotnet run --project src/QRPayCheck.API
```
API: `http://localhost:5000`
API Docs: `http://localhost:5000/scalar`

### 4. Frontend Başlatma
```bash
# Kök dizinden
npm install
npx turbo dev --filter=customer-web
npx turbo dev --filter=restaurant-web
```

## Branching Stratejisi

```
main ─────────────────────────────────── Production
  └── develop ────────────────────────── Development (ana geliştirme)
        ├── feature/xxx ──────────────── Yeni özellikler
        ├── bugfix/xxx ───────────────── Hata düzeltmeleri
        └── hotfix/xxx ───────────────── Acil production düzeltmeleri
```

### Kurallar
- `main` ve `develop` branch'lerine direkt push yasak
- Her iş için `feature/` veya `bugfix/` branch açılır
- PR → `develop` hedefli, Türkçe açıklama ile
- `develop` → `main` merge sadece release zamanı

### Branch İsimlendirme
```
feature/menu-crud
feature/customer-auth
bugfix/order-total-calculation
hotfix/payment-callback-error
```

## Commit Mesajı Formatı

```
<tip>(<kapsam>): <açıklama>

Örnekler:
feat(api): menü CRUD endpoint'leri eklendi
fix(customer-web): sepet toplam hesaplama düzeltildi
chore(docker): Keycloak realm yapılandırması güncellendi
docs(architecture): veritabanı tasarımı eklendi
```

### Tipler
| Tip | Kullanım |
|-----|----------|
| `feat` | Yeni özellik |
| `fix` | Hata düzeltme |
| `docs` | Dokümantasyon |
| `chore` | Build, config, araç değişikliği |
| `refactor` | Kod refactoring |
| `test` | Test ekleme/düzeltme |
| `style` | Kod formatı (işlev değişikliği yok) |

## Proje Yapısı Kuralları

### .NET API
- **Controller:** Sadece request/response mapping, iş mantığı yok
- **Application:** Wolverine handler'lar, FluentValidation, Mapster profilleri
- **Domain:** Entity, Value Object — framework bağımlılığı yok
- **Infrastructure:** DB, external service integration

### React Frontend
- **Sayfa bazlı klasör yapısı:** `src/pages/MenuManagement/`
- **Paylaşılan bileşenler:** `packages/ui/`
- **API çağrıları:** `packages/api-client/` (auto-generated)
- **State management:** Zustand veya React Query (server state)

## Ortam Değişkenleri

### API (.env veya appsettings)
```
DATABASE_URL=Host=localhost;Database=qrpaycheck;Username=postgres;Password=xxx
KEYCLOAK_URL=http://localhost:8080
KEYCLOAK_REALM=qrpaycheck
IYZICO_API_KEY=sandbox-xxx
IYZICO_SECRET_KEY=sandbox-xxx
```

### Frontend (.env)
```
VITE_API_URL=http://localhost:5000/api/v1
VITE_KEYCLOAK_URL=http://localhost:8080
VITE_KEYCLOAK_REALM=qrpaycheck
VITE_KEYCLOAK_CLIENT_ID=customer-app
```
