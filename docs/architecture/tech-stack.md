# Teknoloji Kararları

## Özet Tablo

| Katman | Teknoloji | Versiyon | Gerekçe |
|--------|-----------|----------|---------|
| Backend API | .NET Core | 8.0+ | Performans, tip güvenliği, Clean Architecture uyumu |
| Veritabanı | PostgreSQL | 16+ | Açık kaynak, JSON desteği, row-level security |
| ORM | Entity Framework Core | 8.0+ | .NET ekosistemi ile doğal uyum, migration desteği |
| Auth | Keycloak | 24+ | Self-hosted OAuth2/OIDC, SMS flow desteği, rol yönetimi |
| Ödeme | iyzico | — | Türkiye odaklı, 3D Secure, marketplace desteği |
| Realtime | SignalR | — | .NET native, WebSocket/SSE fallback |
| Mobil | React Native | 0.74+ | Tek codebase ile iOS + Android, React bilgi birikimi |
| Web Frontend | React + Vite | React 18+ | Hızlı build, HMR, ekosistem genişliği |
| Monorepo | Turborepo | 2.x | JS workspace yönetimi, cache, paralel build |
| Container | Docker + Compose | — | Geliştirme ve prod ortam tutarlılığı |
| Reverse Proxy | Nginx | — | Performans, SSL termination, static serving |

## Backend — .NET Core 8

### Neden .NET Core?
- **Performans:** TechEmpower benchmark'larında en hızlı framework'lerden biri
- **Tip güvenliği:** Compile-time hata yakalama, refactoring kolaylığı
- **Clean Architecture:** Katmanlı mimari için ideal ekosistem (MediatR, FluentValidation)
- **SignalR:** Native realtime desteği, ek kütüphane gerekmez

### Kullanılacak NuGet Paketleri
| Paket | Amaç |
|-------|-------|
| MediatR | CQRS pattern — Command/Query ayrımı |
| FluentValidation | Request validasyonu |
| AutoMapper | DTO ↔ Entity mapping |
| Serilog | Structured logging |
| Swashbuckle | OpenAPI/Swagger dokümantasyonu |
| Npgsql.EntityFrameworkCore | PostgreSQL EF Core provider |
| Microsoft.AspNetCore.SignalR | Realtime communication |

## Veritabanı — PostgreSQL

### Neden PostgreSQL?
- Açık kaynak, lisans maliyeti yok
- `jsonb` tipi ile esnek menü/opsiyon verisi saklama
- Row-Level Security ile multi-tenancy güvenlik katmanı
- Full-text search desteği (menü arama)
- Excellent .NET/EF Core desteği

## Auth — Keycloak (Self-Hosted)

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

## Frontend — React (Vite) + React Native

### Monorepo Avantajları
- `packages/ui`: Paylaşılan UI component'leri (web + mobile)
- `packages/shared-types`: API ile senkron TypeScript tipleri
- `packages/api-client`: OpenAPI'den auto-generate edilen client
- Tek `turbo build` ile tüm uygulamalar build edilir

### React Native Seçim Gerekçesi
- React bilgisi direkt transferable
- Web ile component/logic paylaşımı mümkün
- Expo ile hızlı prototipleme
- OTA updates (CodePush)

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
