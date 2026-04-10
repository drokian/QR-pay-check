# Teknoloji Kararları

## Özet Tablo

| Katman | Teknoloji | Versiyon | Gerekçe |
|--------|-----------|----------|---------|
| Backend API | .NET Core | 10.0 (LTS) | Performans, tip güvenliği, Clean Architecture uyumu |
| Veritabanı | PostgreSQL | 18+ | Açık kaynak, JSON desteği, row-level security |
| ORM | Entity Framework Core | 10.0.x | .NET ekosistemi ile doğal uyum, migration desteği |
| Auth | Keycloak | 26+ | Self-hosted OAuth2/OIDC, SMS flow desteği, rol yönetimi |
| Ödeme | iyzico | — | Türkiye odaklı, 3D Secure, marketplace desteği |
| Realtime | SignalR | — | .NET native, WebSocket/SSE fallback |
| Mobil | React Native | 0.85+ | Tek codebase ile iOS + Android, New Architecture varsayılan |
| Web Frontend | React + Vite | React 19.2 / Vite 8.x | Hızlı build, HMR, ekosistem genişliği |
| Monorepo | Turborepo | 2.x | JS workspace yönetimi, cache, paralel build |
| Container | Docker + Compose | — | Geliştirme ve prod ortam tutarlılığı |
| Reverse Proxy | Nginx | — | Performans, SSL termination, static serving |

## Backend — .NET Core 10

### Neden .NET Core?
- **Performans:** TechEmpower benchmark'larında en hızlı framework'lerden biri
- **Tip güvenliği:** Compile-time hata yakalama, refactoring kolaylığı
- **Clean Architecture:** Katmanlı mimari için ideal ekosistem (Wolverine, FluentValidation)
- **SignalR:** Native realtime desteği, ek kütüphane gerekmez
- **.NET 10 LTS:** Kasım 2028'e kadar uzun vadeli destek

### Kullanılacak NuGet Paketleri

| Paket | Versiyon | Amaç |
|-------|----------|-------|
| Wolverine | latest | CQRS pattern — Command/Query/Event işleme (Wolverine.Http ile controller entegrasyonu) |
| FluentValidation | 12.1.1 | Request validasyonu |
| Mapster | latest | DTO ↔ Entity mapping (açık kaynak, AutoMapper alternatifi) |
| Serilog | 4.3.1 | Structured logging |
| Microsoft.AspNetCore.OpenApi | 10.x | Native OpenAPI/Swagger dokümantasyonu (.NET 10 built-in) |
| Scalar.AspNetCore | latest | Modern API dokümantasyon UI (Swashbuckle yerine) |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.1 | PostgreSQL EF Core provider |
| Microsoft.AspNetCore.SignalR | — | Realtime communication |

### Wolverine vs MediatR

MediatR, 2025'te ticari lisansa geçti (RPL-1.5). Wolverine açık kaynak ve daha yetenekli bir alternatif:
- **In-process messaging:** MediatR ile aynı CQRS pattern desteği
- **Wolverine.Http:** Controller yerine doğrudan endpoint tanımı
- **Durable messaging:** Outbox pattern, mesaj kalıcılığı
- **Saga/stateful workflow:** Uzun süren iş akışları için

### Mapster vs AutoMapper

AutoMapper, 2025'te ticari lisansa geçti (RPL-1.5). Mapster açık kaynak ve daha hızlı:
- Benchmark'larda AutoMapper'dan 2-3x daha hızlı
- Source generator desteği ile zero-runtime-reflection mapping
- Daha az konfigürasyon, daha sezgisel API

### Microsoft.AspNetCore.OpenApi + Scalar vs Swashbuckle

.NET 9'dan itibaren `Microsoft.AspNetCore.OpenApi` native olarak dahil. Swashbuckle artık varsayılan değil ve aktif gelişimi yavaşladı:
- **Microsoft.AspNetCore.OpenApi:** Minimal API ve controller routing tam desteği, .NET 10 ile birlikte gelişiyor
- **Scalar UI:** Swashbuckle UI'ın modern alternatifi, daha iyi UX, aktif geliştirme

## Veritabanı — PostgreSQL 18

### Neden PostgreSQL?
- Açık kaynak, lisans maliyeti yok
- `jsonb` tipi ile esnek menü/opsiyon verisi saklama
- Row-Level Security ile multi-tenancy güvenlik katmanı
- Full-text search desteği (menü arama)
- Virtual generated columns (PG 18 yenilik)
- Excellent .NET/EF Core desteği

## Auth — Keycloak 26 (Self-Hosted)

### Neden Keycloak?
- Self-hosted: Veri kontrolü tamamen bizde
- OAuth2 + OIDC standardı
- Custom Authentication Flow: SMS OTP akışı için genişletilebilir
- Realm bazlı izolasyon
- Admin Console ile kullanıcı/rol yönetimi
- Docker ile kolay kurulum

### Neden Auth0 Değil?
- Kullanıcı sayısı arttıkça maliyetli
- Veri yurt dışında saklanıyor (KVKK riski)

## Ödeme — iyzico

### Neden iyzico?
- Türkiye'de en yaygın ödeme altyapısı
- 3D Secure zorunlu desteği
- Taksit/kampanya yönetimi
- Marketplace API (hesap bölme için ideal)
- Kolay sandbox ortamı

## Realtime — SignalR

### Neden SignalR?
- .NET Core ile native entegrasyon
- WebSocket → Server-Sent Events → Long Polling otomatik fallback
- Hub pattern ile kolay grup yönetimi (masalara özel bildirim)
- Scale-out: Redis backplane ile horizontal scaling

## Frontend — React 19 (Vite 8) + React Native 0.85

### React 19 Yenilikleri
- Server Components desteği
- Actions API (form handling kolaylaşıyor)
- `use()` hook ile promise/context okuma
- `forwardRef` kaldırıldı — ref doğrudan prop olarak geçiliyor

### React Native 0.85 — New Architecture
- Fabric renderer ve TurboModules artık varsayılan
- JSI (JavaScript Interface) ile native bridge performansı
- Hermes V1 engine

### Monorepo Avantajları
- `packages/ui`: Paylaşılan UI component'leri (web + mobile)
- `packages/shared-types`: API ile senkron TypeScript tipleri
- `packages/api-client`: OpenAPI'den auto-generate edilen client
- Tek `turbo build` ile tüm uygulamalar build edilir

## Deployment — Docker + VPS

### Neden Cloud Provider (Azure/AWS) Değil?
- MVP aşamasında maliyet kontrolü
- Basit yapı: tek VPS üzerinde Docker Compose
- İlerleyen aşamada Kubernetes'e geçiş kapısı açık

### Docker Compose Servisleri

| Servis | Port | Açıklama |
|--------|------|----------|
| `api` | 5000 | .NET Core Web API |
| `db` | 5432 | PostgreSQL |
| `keycloak` | 8080 | Keycloak Auth Server |
| `nginx` | 80/443 | Reverse proxy + static files |
