# QR Sipariş Akışı

## Kullanıcı Hikayesi

> Bir restoran müşterisi olarak, masadaki QR kodu okutarak menüyü görüntülemek ve sipariş vermek istiyorum, böylece garson beklemeden hızlıca sipariş verebiliyorum.

## Akış Diyagramı

```
┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐
│ QR Okut  │────>│ Telefon  │────>│  Menüyü  │────>│ Siparişi │
│          │     │ Doğrula  │     │ Görüntüle│     │ Oluştur  │
└──────────┘     └──────────┘     └──────────┘     └──────────┘
                                                         │
┌──────────┐     ┌──────────┐     ┌──────────┐           │
│ Sipariş  │<────│ Hazırla  │<────│ Restoran │<──────────┘
│  Servis  │     │          │     │  Onayı   │    (SignalR bildirim)
└──────────┘     └──────────┘     └──────────┘
```

## Detaylı Adımlar

### 1. QR Kod Okutma
- Müşteri masadaki QR kodu telefonuyla okuttur
- QR kod URL formatı: `https://app.qrpaycheck.com/qr/{code}`
- Sistem deep link ile mobil uygulamayı açar veya web'e yönlendirir

### 2. Telefon Doğrulama
- İlk kullanımda telefon numarası istenir
- SMS OTP gönderilir → doğrulama → JWT alınır
- Daha önce giriş yaptıysa token refresh ile devam

### 3. Oturum Başlatma
- `GET /api/v1/qr/{code}` → masa bilgisi ve restoran detayı döner
- `POST /api/v1/sessions` → masa oturumu başlatılır
- Aynı masada aktif oturum varsa, o oturuma katılım sağlanır

### 4. Menü Görüntüleme
- `GET /api/v1/sessions/{id}/menu` → kategorilerle birlikte menü
- Ürün filtreleme: kategori, alerjen, fiyat aralığı
- Ürün detay: fotoğraf, açıklama, seçenekler, hazırlanma süresi

### 5. Sepet & Sipariş Oluşturma
- Müşteri ürünleri sepete ekler (client-side state)
- Opsiyonları seçer (boyut, ekstra malzeme vb.)
- Sipariş notu ekleyebilir
- `POST /api/v1/sessions/{id}/orders` ile sipariş gönderilir

### 6. Sipariş Bildirimi (Realtime)
- Sipariş oluşturulunca SignalR ile restoran paneline push notification
- Restoran onaylar veya reddeder
- Durum değişiklikleri: `pending` → `confirmed` → `preparing` → `ready` → `served`
- Her durum değişikliğinde müşteriye SignalR bildirimi

### 7. Ek Sipariş
- Oturum aktif olduğu sürece müşteri yeni sipariş verebilir
- Her sipariş ayrı bir Order kaydı olarak tutulur
- Tüm siparişler Session altında gruplanır

## API Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/v1/qr/{code}` | QR kod bilgisi → masa + restoran |
| POST | `/api/v1/sessions` | Oturum başlat |
| GET | `/api/v1/sessions/{id}` | Oturum detayı |
| GET | `/api/v1/sessions/{id}/menu` | Menü listesi |
| GET | `/api/v1/menu-items/{id}` | Ürün detayı + seçenekler |
| POST | `/api/v1/sessions/{id}/orders` | Sipariş oluştur |
| GET | `/api/v1/sessions/{id}/orders` | Oturumdaki siparişler |
| GET | `/api/v1/orders/{id}` | Sipariş detayı |

## Ekranlar

### Müşteri Tarafı
1. **QR Tarama** — Kamera veya URL yönlendirme
2. **Telefon Doğrulama** — Numara girişi + OTP
3. **Menü Listesi** — Kategoriler + ürünler
4. **Ürün Detay** — Seçenekler, miktar
5. **Sepet** — Seçilen ürünler, toplam, sipariş notu
6. **Sipariş Takip** — Durum güncellemeleri (realtime)

### Restoran Tarafı
1. **Aktif Siparişler** — Kanban board (pending → preparing → ready → served)
2. **Sipariş Detay** — Masa, ürünler, notlar
3. **Bildirimler** — Yeni sipariş alert
