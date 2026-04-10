# QR Pay Check

Restoran ve kafeler için QR kod tabanlı sipariş ve ödeme sistemi.

## Konsept

Müşteriler masadaki QR kodu okutarak menüyü görüntüler, sipariş verir ve ödeme yapar. Restoran sahipleri menü, masa ve siparişleri yönetim panelinden takip eder.

## Teknoloji Stack

| Katman | Teknoloji |
|--------|-----------|
| Backend | .NET Core 8, Clean Architecture, MediatR |
| Veritabanı | PostgreSQL + EF Core |
| Auth | Keycloak (OAuth2/OIDC) |
| Ödeme | iyzico |
| Realtime | SignalR |
| Web Frontend | React + Vite |
| Mobil | React Native |
| Monorepo | Turborepo |
| Deployment | Docker + Nginx |

## Proje Yapısı

```
apps/
  api/               # .NET Core Web API
  customer-web/       # Müşteri web uygulaması
  customer-mobile/    # Müşteri mobil uygulaması
  restaurant-web/     # Restoran yönetim web
  restaurant-mobile/  # Restoran yönetim mobil
  admin-web/          # Platform admin paneli
packages/
  ui/                 # Paylaşılan UI bileşenleri
  api-client/         # Auto-generated API client
  shared-types/       # Ortak TypeScript tipleri
  utils/              # Yardımcı fonksiyonlar
infrastructure/
  docker/             # Docker Compose yapılandırması
docs/                 # Proje dokümantasyonu
```

## Hızlı Başlangıç

```bash
# Docker servisleri başlat
cd infrastructure/docker
docker-compose -f docker-compose.dev.yml up -d

# API başlat
cd apps/api
dotnet run --project src/QRPayCheck.API

# Frontend başlat
npm install
npx turbo dev
```

## Dokümantasyon

- [Mimari Genel Bakış](docs/architecture/overview.md)
- [Teknoloji Kararları](docs/architecture/tech-stack.md)
- [Veritabanı Tasarımı](docs/architecture/database-design.md)
- [API Tasarımı](docs/architecture/api-design.md)
- [Auth Akışı](docs/architecture/auth-flow.md)
- [Geliştirici Rehberi](docs/development-guide.md)
