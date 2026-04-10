# Veritabanı Tasarımı

## ER Diyagramı (Basitleştirilmiş)

```
┌──────────┐     ┌──────────┐     ┌──────────────┐
│  Tenant  │────<│  Branch  │────<│     Table    │
│(Restoran)│     │  (Şube)  │     │              │
└──────────┘     └──────────┘     └───────┬──────┘
                      │                   │
                      │             ┌─────▼──────┐
                 ┌────▼───────┐     │  QRCode    │
                 │   Menu     │     └────────────┘
                 └────┬───────┘           │
                      │            ┌──────▼────────┐
                 ┌────▼───────┐    │   Session     │
                 │  Category  │    │ (Masa Oturum) │
                 └────┬───────┘    └──────┬────────┘
                      │                   │
                 ┌────▼───────┐    ┌──────▼────────┐
                 │  MenuItem  │    │    Order      │
                 └────┬───────┘    └──────┬────────┘
                      │                   │
                 ┌────▼───────────┐┌──────▼────────┐
                 │MenuItemOption  ││  OrderItem    │
                 └────────────────┘└──────┬────────┘
                                          │
                                    ┌─────▼──────┐
                                    │  Payment   │
                                    └─────┬──────┘
                                          │
                                   ┌──────▼───────┐
                                   │ PaymentSplit │
                                   └──────────────┘
```

## Tablo Detayları

### Tenants (Restoranlar)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | Primary key |
| Name | varchar(200) | Restoran adı |
| Slug | varchar(100) | URL-friendly isim (unique) |
| LogoUrl | varchar(500) | Logo dosya yolu |
| Phone | varchar(20) | İletişim telefonu |
| Email | varchar(200) | İletişim e-posta |
| TaxNumber | varchar(20) | Vergi numarası |
| IsActive | bool | Aktif/pasif durumu |
| CreatedAt | timestamptz | Oluşturulma tarihi |
| UpdatedAt | timestamptz | Güncellenme tarihi |

### Branches (Şubeler)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| TenantId | UUID (FK) | Bağlı restoran |
| Name | varchar(200) | Şube adı |
| Address | text | Adres |
| City | varchar(100) | Şehir |
| District | varchar(100) | İlçe |
| Latitude | decimal(9,6) | Enlem |
| Longitude | decimal(9,6) | Boylam |
| Phone | varchar(20) | Şube telefonu |
| IsActive | bool | |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

### Tables (Masalar)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| BranchId | UUID (FK) | Bağlı şube |
| TableNumber | varchar(20) | Masa numarası (ör: "A1", "12") |
| Capacity | int | Kişi kapasitesi |
| Section | varchar(50) | Bölge (ör: "Bahçe", "İç Mekan") |
| IsActive | bool | |
| CreatedAt | timestamptz | |

### QRCodes

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| TableId | UUID (FK) | Bağlı masa |
| Code | varchar(100) | Unique QR kodu değeri |
| QRImageUrl | varchar(500) | QR kod görseli URL |
| IsActive | bool | Aktif/pasif (yeniden üretim için) |
| CreatedAt | timestamptz | |

### Users

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| KeycloakId | varchar(100) | Keycloak user ID (unique) |
| TenantId | UUID (FK, nullable) | Bağlı restoran (müşteri için null) |
| Phone | varchar(20) | Telefon numarası (unique) |
| FullName | varchar(200) | Ad soyad |
| Role | varchar(50) | platform-admin, restaurant-owner, restaurant-staff, customer |
| IsActive | bool | |
| CreatedAt | timestamptz | |
| LastLoginAt | timestamptz | |

### Menus

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| BranchId | UUID (FK) | Bağlı şube |
| Name | varchar(200) | Menü adı (ör: "Ana Menü", "İçecek Menüsü") |
| IsActive | bool | |
| SortOrder | int | Sıralama |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

### Categories

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| MenuId | UUID (FK) | Bağlı menü |
| Name | varchar(200) | Kategori adı (ör: "Başlangıçlar", "Ana Yemekler") |
| Description | text | Açıklama |
| ImageUrl | varchar(500) | Kategori görseli |
| SortOrder | int | Sıralama |
| IsActive | bool | |

### MenuItems

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| CategoryId | UUID (FK) | Bağlı kategori |
| Name | varchar(200) | Ürün adı |
| Description | text | Açıklama |
| Price | decimal(10,2) | Fiyat (TL) |
| ImageUrl | varchar(500) | Ürün görseli |
| IsAvailable | bool | Stokta var mı |
| PreparationTime | int | Tahmini hazırlanma süresi (dakika) |
| Allergens | jsonb | Alerjen bilgileri |
| SortOrder | int | |
| IsActive | bool | |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

### MenuItemOptions (Ürün Seçenekleri)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| MenuItemId | UUID (FK) | Bağlı ürün |
| GroupName | varchar(100) | Seçenek grubu (ör: "Boyut", "Ekstra") |
| Name | varchar(200) | Seçenek adı (ör: "Büyük Boy", "Ekstra Peynir") |
| PriceModifier | decimal(10,2) | Fiyat farkı (+/- TL) |
| IsDefault | bool | Varsayılan seçili mi |
| IsRequired | bool | Zorunlu seçim mi |
| MaxSelections | int | Maksimum seçim sayısı (grup bazlı) |
| SortOrder | int | |

### Sessions (Masa Oturumları)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| TableId | UUID (FK) | Bağlı masa |
| QRCodeId | UUID (FK) | Kullanılan QR kod |
| Status | varchar(20) | `active`, `payment_pending`, `closed` |
| GuestCount | int | Misafir sayısı |
| OpenedAt | timestamptz | Oturum başlangıcı |
| ClosedAt | timestamptz | Oturum kapanışı |
| TotalAmount | decimal(10,2) | Toplam tutar |

### Orders (Siparişler)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| SessionId | UUID (FK) | Bağlı oturum |
| UserId | UUID (FK) | Sipariş veren müşteri |
| OrderNumber | varchar(20) | Sipariş numarası (gün bazlı sıralı) |
| Status | varchar(20) | `pending`, `confirmed`, `preparing`, `ready`, `served`, `cancelled` |
| Note | text | Sipariş notu |
| SubTotal | decimal(10,2) | Ara toplam |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

### OrderItems (Sipariş Kalemleri)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| OrderId | UUID (FK) | Bağlı sipariş |
| MenuItemId | UUID (FK) | Bağlı menü ürünü |
| Quantity | int | Adet |
| UnitPrice | decimal(10,2) | Birim fiyat (sipariş anındaki) |
| TotalPrice | decimal(10,2) | Toplam (birim × adet + seçenekler) |
| SelectedOptions | jsonb | Seçilen opsiyonlar snapshot |
| Note | text | Kalem notu (ör: "az pişmiş") |

### Payments (Ödemeler)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| SessionId | UUID (FK) | Bağlı oturum |
| UserId | UUID (FK) | Ödeme yapan müşteri |
| Amount | decimal(10,2) | Ödenen tutar |
| Status | varchar(20) | `pending`, `processing`, `completed`, `failed`, `refunded` |
| PaymentMethod | varchar(20) | `credit_card`, `debit_card` |
| IyzicoPaymentId | varchar(100) | iyzico referans ID |
| IyzicoConversationId | varchar(100) | iyzico conversation ID |
| CreatedAt | timestamptz | |
| CompletedAt | timestamptz | |

### PaymentSplits (Hesap Bölme)

| Kolon | Tip | Açıklama |
|-------|-----|----------|
| Id | UUID (PK) | |
| PaymentId | UUID (FK) | Bağlı ödeme kaydı |
| UserId | UUID (FK) | Ödeme yapan kişi |
| SplitType | varchar(20) | `equal`, `by_item`, `custom_amount` |
| Amount | decimal(10,2) | Bu kişinin payı |
| OrderItemIds | jsonb | Seçilen sipariş kalemleri (by_item için) |
| Status | varchar(20) | `pending`, `completed`, `failed` |

## İndeksler

```sql
-- Sık sorgulanan FK'lar
CREATE INDEX IX_Branches_TenantId ON Branches(TenantId);
CREATE INDEX IX_Tables_BranchId ON Tables(BranchId);
CREATE INDEX IX_Sessions_TableId_Status ON Sessions(TableId, Status);
CREATE INDEX IX_Orders_SessionId ON Orders(SessionId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Payments_SessionId ON Payments(SessionId);
CREATE INDEX IX_MenuItems_CategoryId ON MenuItems(CategoryId);

-- Unique constraints
CREATE UNIQUE INDEX UX_Tenants_Slug ON Tenants(Slug);
CREATE UNIQUE INDEX UX_QRCodes_Code ON QRCodes(Code);
CREATE UNIQUE INDEX UX_Users_Phone ON Users(Phone);
CREATE UNIQUE INDEX UX_Users_KeycloakId ON Users(KeycloakId);
```

## Multi-Tenancy

Tüm tenant-bağımlı tablolarda (Branches, Tables, Menus, vb.) EF Core Global Query Filter ile otomatik `TenantId` filtreleme uygulanır:

```csharp
// BaseEntity'den türeyen tüm tenant-aware entity'ler
modelBuilder.Entity<Branch>().HasQueryFilter(b => b.TenantId == _currentTenant.Id);
```
