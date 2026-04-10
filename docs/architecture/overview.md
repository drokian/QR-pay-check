# Mimari Genel Bakış

## Sistem Bileşenleri

```
┌─────────────────────────────────────────────────────────────────────┐
│                        İSTEMCİLER (Clients)                        │
├──────────┬──────────┬──────────┬──────────┬─────────────────────────┤
│ Müşteri  │ Müşteri  │ Restoran │ Restoran │      Admin Web          │
│  Mobil   │   Web    │  Mobil   │   Web    │   (Platform Yönetim)    │
│(RN)      │(React)   │(RN)      │(React)   │      (React)            │
└────┬─────┴────┬─────┴────┬─────┴────┬─────┴────────────┬────────────┘
     │          │          │          │                   │
     └──────────┴──────────┴──────────┴───────────────────┘
                             │
                    ┌────────▼────────┐
                    │   API Gateway   │
                    │  (.NET Core)    │
                    └────────┬────────┘
                             │
          ┌──────────────────┼──────────────────┐
          │                  │                  │
   ┌──────▼──────┐   ┌──────▼──────┐   ┌──────▼──────┐
   │  Keycloak   │   │ PostgreSQL  │   │  SignalR     │
   │  (Auth)     │   │  (DB)       │   │  (Realtime)  │
   └─────────────┘   └─────────────┘   └──────────────┘
                             │
                    ┌────────▼────────┐
                    │    iyzico       │
                    │  (Ödeme)        │
                    └─────────────────┘
```

## Katmanlı Mimari (.NET Core — Clean Architecture)

```
┌─────────────────────────────────┐
│         QRPayCheck.API          │  ← Controllers, Middleware, Filters
├─────────────────────────────────┤
│     QRPayCheck.Application      │  ← Use Cases, CQRS (MediatR), DTOs
├─────────────────────────────────┤
│       QRPayCheck.Domain         │  ← Entities, Value Objects, Events
├─────────────────────────────────┤
│   QRPayCheck.Infrastructure     │  ← EF Core, Keycloak, iyzico, SignalR
├─────────────────────────────────┤
│       QRPayCheck.Shared         │  ← Exceptions, Constants, Helpers
└─────────────────────────────────┘
```

### Bağımlılık Yönü

- **API** → Application, Infrastructure, Shared
- **Application** → Domain, Shared
- **Infrastructure** → Application, Domain, Shared
- **Domain** → Shared (minimal)
- **Shared** → Hiçbir şeye bağımlı değil

## İletişim Akışları

### Sipariş Akışı
1. Müşteri QR kodu okutarak `customer-web` veya `customer-mobile` açar
2. QR'daki masa bilgisi ile API'ye `POST /sessions` isteği gider → oturum başlar
3. Müşteri menüyü görüntüler (`GET /menus/{menuId}`)
4. Sipariş oluşturur (`POST /orders`)
5. SignalR üzerinden restoran tarafına anlık bildirim gider
6. Restoran staff sipariş durumunu günceller → müşteriye SignalR ile bildirim

### Ödeme Akışı
1. Müşteri "Hesap İste" → `GET /sessions/{id}/bill`
2. Hesap bölme seçenekleri sunulur
3. Ödeme başlatılır → iyzico 3D Secure akışı
4. iyzico callback → API ödeme durumunu günceller
5. SignalR ile restoran tarafına ödeme bildirimi

## Multi-Tenancy Stratejisi

- **Tek veritabanı**, `TenantId` ile veri izolasyonu (row-level filtering)
- EF Core Global Query Filter ile otomatik tenant filtreleme
- Her request'te JWT'den `TenantId` çıkarılır

## Deployment Mimarisi

```
┌─────────────────────────── VPS ───────────────────────────┐
│                                                           │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │  Nginx   │  │  .NET    │  │ Keycloak │  │PostgreSQL│   │
│  │ (Reverse │──│  API     │  │          │  │          │   │
│  │  Proxy)  │  │ Container│  │ Container│  │Container │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
│                                                           │
│  ┌──────────────────────────────────────────────────────┐ │
│  │  Static Files (React builds) — Nginx ile serve       │ │
│  └──────────────────────────────────────────────────────┘ │
└───────────────────────────────────────────────────────────┘
```

- Docker Compose ile tüm servisler orchestrate edilir
- Nginx reverse proxy: API routing + static file serving
- SSL: Let's Encrypt (certbot)
