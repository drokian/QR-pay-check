# Kimlik Doğrulama & Yetkilendirme Akışı

## Genel Bakış

- **Auth Provider:** Keycloak (self-hosted, Docker)
- **Protokol:** OAuth2 / OpenID Connect
- **Token:** JWT (Access Token + Refresh Token)
- **Müşteri Auth:** Telefon + SMS OTP
- **Restoran/Admin Auth:** Email + Şifre

## Keycloak Yapılandırması

### Realm: `qrpaycheck`

### Client'lar

| Client ID | Tip | Kullanıcı |
|-----------|-----|-----------|
| `customer-app` | Public | Müşteri mobil + web |
| `restaurant-app` | Public | Restoran sahibi mobil + web |
| `admin-app` | Public | Platform admin web |
| `api-service` | Confidential | Backend API (token introspection) |

### Roller

| Rol | Açıklama | Yetkiler |
|-----|----------|----------|
| `platform-admin` | Platform yöneticisi | Tüm erişim |
| `restaurant-owner` | Restoran sahibi | Kendi restoranı: menü, masa, sipariş, ödeme, personel |
| `restaurant-staff` | Restoran personeli | Sipariş yönetimi, masa durumu |
| `customer` | Müşteri | Sipariş verme, ödeme yapma |

## Akış Diyagramları

### 1. Müşteri — SMS OTP Akışı

```
Müşteri              API                Keycloak           SMS Provider
  │                   │                    │                    │
  │ POST /auth/send-otp                    │                    │
  │ {phone: "05xx"}   │                    │                    │
  │──────────────────>│                    │                    │
  │                   │  OTP üret & kaydet │                    │
  │                   │───────────────────>│                    │
  │                   │                    │  SMS gönder        │
  │                   │                    │───────────────────>│
  │                   │                    │                    │
  │  200 OK           │                    │                    │
  │<──────────────────│                    │                    │
  │                   │                    │                    │
  │ POST /auth/verify-otp                  │                    │
  │ {phone, otp}      │                    │                    │
  │──────────────────>│                    │                    │
  │                   │  OTP doğrula       │                    │
  │                   │───────────────────>│                    │
  │                   │                    │                    │
  │                   │  Kullanıcı yoksa   │                    │
  │                   │  otomatik create   │                    │
  │                   │───────────────────>│                    │
  │                   │                    │                    │
  │                   │  JWT döndür        │                    │
  │                   │<───────────────────│                    │
  │  {accessToken,    │                    │                    │
  │   refreshToken}   │                    │                    │
  │<──────────────────│                    │                    │
```

### 2. Restoran Sahibi — Email/Şifre Akışı

```
Restoran Sahibi      Frontend            Keycloak
  │                   │                    │
  │  Email + Şifre    │                    │
  │──────────────────>│                    │
  │                   │  Authorization     │
  │                   │  Code Flow (PKCE)  │
  │                   │───────────────────>│
  │                   │                    │
  │                   │  auth code         │
  │                   │<───────────────────│
  │                   │                    │
  │                   │  POST /token       │
  │                   │  (code + verifier) │
  │                   │───────────────────>│
  │                   │                    │
  │                   │  JWT               │
  │                   │<───────────────────│
  │  Giriş başarılı   │                    │
  │<──────────────────│                    │
```

## JWT Token Yapısı

### Access Token Claims

```json
{
  "sub": "user-uuid",
  "realm_access": {
    "roles": ["customer"]
  },
  "tenant_id": "restaurant-uuid",
  "phone": "05xxxxxxxxx",
  "name": "Kullanıcı Adı",
  "exp": 1234567890,
  "iss": "https://auth.qrpaycheck.com/realms/qrpaycheck"
}
```

### Token Süreleri

| Token | Süre |
|-------|------|
| Access Token | 15 dakika |
| Refresh Token | 7 gün |
| OTP Geçerlilik | 5 dakika |
| OTP Deneme Hakkı | 3 kez |

## API Tarafı Yetkilendirme

### Middleware Zinciri

```
Request → JWT Validation → Tenant Resolution → Role Check → Controller
```

### Attribute Bazlı Yetkilendirme

```csharp
[Authorize(Roles = "restaurant-owner,restaurant-staff")]
[HttpPut("orders/{orderId}/status")]
public async Task<IActionResult> UpdateOrderStatus(...)
```

### Tenant İzolasyonu

```csharp
// Her request'te JWT'den TenantId çıkarılır
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
    {
        var tenantId = context.User.FindFirst("tenant_id")?.Value;
        tenantService.Set(tenantId);
    }
}
```

## Güvenlik Önlemleri

- **OTP brute-force koruması:** 5 req/dk/IP, 3 yanlış deneme → 15 dk blok
- **Refresh token rotation:** Her kullanımda yeni refresh token
- **CORS:** Sadece izin verilen origin'ler
- **HTTPS zorunlu** (production)
- **Keycloak Brute Force Detection** aktif
- **Rate limiting** tüm auth endpoint'lerinde
