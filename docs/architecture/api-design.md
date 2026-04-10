# API Tasarım Prensipleri

## Genel Prensipler

- **RESTful** tasarım — kaynak odaklı URL yapısı
- **Clean Architecture** — CQRS pattern (Command/Query ayrımı) Wolverine ile
- **Versiyonlama:** URL bazlı (`/api/v1/...`)
- **Response format:** JSON, tutarlı envelope pattern
- **Dokümantasyon:** Scalar + Microsoft.AspNetCore.OpenApi
- **Auth:** Bearer JWT token (Keycloak)

## Standart Response Envelope

```json
{
  "success": true,
  "data": { ... },
  "errors": null,
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 150
  }
}
```

Hata durumu:
```json
{
  "success": false,
  "data": null,
  "errors": [
    {
      "code": "MENU_ITEM_NOT_FOUND",
      "message": "Menü öğesi bulunamadı.",
      "field": null
    }
  ],
  "meta": null
}
```

## HTTP Status Code Kullanımı

| Kod | Kullanım |
|-----|----------|
| 200 | Başarılı GET, PUT |
| 201 | Başarılı POST (kaynak oluşturma) |
| 204 | Başarılı DELETE |
| 400 | Validasyon hatası |
| 401 | Kimlik doğrulanmamış |
| 403 | Yetki yok |
| 404 | Kaynak bulunamadı |
| 409 | Conflict (ör: duplicate) |
| 422 | Business rule ihlali |
| 500 | Sunucu hatası |

## Endpoint Yapısı

### Naming Convention
- Çoğul isim: `/api/v1/restaurants`, `/api/v1/tables`
- Nested resource: `/api/v1/branches/{branchId}/tables`
- Action endpoint: `/api/v1/orders/{orderId}/confirm` (POST)

### Müşteri API'leri (Customer)

```
POST   /api/v1/auth/send-otp              # SMS OTP gönder
POST   /api/v1/auth/verify-otp            # OTP doğrula → JWT

GET    /api/v1/qr/{code}                  # QR kod → masa bilgisi
POST   /api/v1/sessions                   # Oturum başlat
GET    /api/v1/sessions/{id}              # Oturum detayı

GET    /api/v1/sessions/{id}/menu         # Masanın menüsü
GET    /api/v1/menu-items/{id}            # Ürün detayı

POST   /api/v1/sessions/{id}/orders       # Sipariş oluştur
GET    /api/v1/sessions/{id}/orders       # Oturumdaki siparişler
GET    /api/v1/orders/{id}                # Sipariş detayı

GET    /api/v1/sessions/{id}/bill         # Hesap özeti
POST   /api/v1/sessions/{id}/payments     # Ödeme başlat
POST   /api/v1/payments/{id}/split        # Hesap böl
POST   /api/v1/payments/callback          # iyzico webhook
```

### Restoran Yönetim API'leri (Restaurant)

```
# Auth
POST   /api/v1/auth/login                 # Keycloak login

# Restoran
GET    /api/v1/restaurant/profile         # Restoran profili
PUT    /api/v1/restaurant/profile         # Profil güncelle

# Şube
GET    /api/v1/branches                   # Şube listesi
POST   /api/v1/branches                   # Şube ekle
PUT    /api/v1/branches/{id}              # Şube güncelle

# Masa
GET    /api/v1/branches/{id}/tables       # Masa listesi
POST   /api/v1/branches/{id}/tables       # Masa ekle
PUT    /api/v1/tables/{id}                # Masa güncelle
DELETE /api/v1/tables/{id}                # Masa sil
POST   /api/v1/tables/{id}/generate-qr    # QR kod oluştur

# Menü
GET    /api/v1/branches/{id}/menus        # Menü listesi
POST   /api/v1/menus                      # Menü oluştur
PUT    /api/v1/menus/{id}                 # Menü güncelle
POST   /api/v1/menus/{id}/categories      # Kategori ekle
PUT    /api/v1/categories/{id}            # Kategori güncelle
POST   /api/v1/categories/{id}/items      # Ürün ekle
PUT    /api/v1/menu-items/{id}            # Ürün güncelle
PUT    /api/v1/menu-items/{id}/availability # Stok durumu

# Sipariş Yönetimi
GET    /api/v1/branches/{id}/orders       # Aktif siparişler
PUT    /api/v1/orders/{id}/status         # Sipariş durumu güncelle
GET    /api/v1/branches/{id}/sessions     # Aktif oturumlar

# Ödeme
GET    /api/v1/branches/{id}/payments     # Ödeme listesi
GET    /api/v1/payments/{id}              # Ödeme detayı
```

### Platform Admin API'leri

```
GET    /api/v1/admin/tenants              # Restoran listesi
POST   /api/v1/admin/tenants              # Restoran ekle
PUT    /api/v1/admin/tenants/{id}         # Restoran güncelle
PUT    /api/v1/admin/tenants/{id}/status  # Aktif/pasif

GET    /api/v1/admin/users                # Kullanıcı listesi
GET    /api/v1/admin/dashboard            # İstatistikler
```

## CQRS Pattern (Wolverine)

### Command Örneği
```csharp
// Command
public record CreateOrderCommand(Guid SessionId, List<OrderItemDto> Items, string? Note);

// Handler
public static class CreateOrderHandler
{
    public static async Task<OrderResponse> HandleAsync(
        CreateOrderCommand command,
        IDocumentSession session,
        CancellationToken ct)
    {
        // 1. Validasyon (FluentValidation pipeline)
        // 2. Domain logic
        // 3. Persist
        // 4. Domain event publish (sipariş bildirimi)
        // 5. Return DTO
    }
}
```

### Query Örneği
```csharp
public record GetSessionBillQuery(Guid SessionId);

public static class GetSessionBillHandler
{
    public static async Task<BillResponse> HandleAsync(
        GetSessionBillQuery query,
        IQuerySession session,
        CancellationToken ct)
    {
        // Query logic
    }
}
```

## Pagination

Liste endpoint'leri standart query parametreleri alır:

```
GET /api/v1/branches/{id}/orders?page=1&pageSize=20&status=pending&sortBy=createdAt&sortDir=desc
```

## Rate Limiting

| Endpoint Grubu | Limit |
|----------------|-------|
| Auth (OTP) | 5 req/dakika/IP |
| Genel API | 100 req/dakika/user |
| Webhook | IP whitelist |
