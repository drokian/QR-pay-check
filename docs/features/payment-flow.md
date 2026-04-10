# Ödeme & Hesap Bölme Akışı

## Kullanıcı Hikayesi

> Bir restoran müşterisi olarak, yemeğimi bitirdikten sonra QR kodu tekrar okutarak hesabımı görmek ve ödemek istiyorum. Hesabı arkadaşlarımla bölmek isteyebilirim.

## Ödeme Seçenekleri

| Seçenek | Açıklama |
|---------|----------|
| **Tamamını Öde** | Bir kişi tüm hesabı öder |
| **Eşit Böl** | Toplam tutar kişi sayısına eşit bölünür |
| **Ürün Bazlı Böl** | Her kişi kendi siparişini seçip öder |
| **Manuel Tutar** | Her kişi istediği tutarı girer |

## Akış Diyagramı

```
┌──────────┐     ┌──────────┐     ┌──────────────┐     ┌──────────┐
│  Hesap   │────>│  Bölme   │────>│   iyzico     │────>│  Ödeme   │
│  Görüntüle│    │ Seçeneği │     │  3D Secure   │     │  Tamamla │
└──────────┘     └──────────┘     └──────────────┘     └──────────┘
                                                             │
                                                    ┌────────▼───────┐
                                                    │ Restoran Panel │
                                                    │  (bildirim)    │
                                                    └────────────────┘
```

## Detaylı Adımlar

### 1. Hesap Görüntüleme
- `GET /api/v1/sessions/{id}/bill` → tüm siparişlerin özeti
- Response: sipariş kalemleri, fiyatlar, toplam tutar

### 2. Ödeme Yöntemi Seçimi
- Şimdilik sadece kredi/banka kartı (iyzico)
- İleride: nakit (garson onaylı), cüzdan bakiyesi

### 3. Hesap Bölme Akışı

#### a) Tamamını Öde
- Tek kişi toplam tutarı öder
- Direkt iyzico ödeme akışına geçilir

#### b) Eşit Böl
- Kişi sayısı girilir
- Sistem toplam tutarı eşit böler
- Her kişi kendi payını ayrı öder
- Tüm paylar ödenince oturum kapanır

#### c) Ürün Bazlı Böl
- Her kişi kendi sipariş ettiği ürünleri seçer
- Seçilmemiş ürün kalmayana kadar devam eder
- Her kişi seçtiği ürünlerin toplamını öder

#### d) Manuel Tutar Böl
- Her kişi ödemek istediği tutarı girer
- Toplam tutarı geçemez
- Kalan tutar gösterilir
- Tüm tutar karşılanınca tamamlanır

### 4. iyzico Ödeme Akışı

```
Müşteri              API                iyzico
  │                   │                    │
  │ POST /payments    │                    │
  │ {amount, card}    │                    │
  │──────────────────>│                    │
  │                   │ Create Payment     │
  │                   │───────────────────>│
  │                   │                    │
  │                   │ 3D Secure HTML     │
  │                   │<───────────────────│
  │ 3D Secure sayfası │                    │
  │<──────────────────│                    │
  │                   │                    │
  │ 3D Secure onay    │                    │
  │──────────────────>│                    │
  │                   │ Confirm Payment    │
  │                   │───────────────────>│
  │                   │                    │
  │                   │ Payment Result     │
  │                   │<───────────────────│
  │                   │                    │
  │ Ödeme sonucu      │                    │
  │<──────────────────│                    │
  │                   │                    │
  │                   │ Webhook callback   │
  │                   │<───────────────────│
  │                   │ (final durum)      │
```

### 5. Ödeme Sonrası
- Ödeme başarılı → SignalR ile restoran paneline bildirim
- Tüm siparişler ödendi → Session status: `closed`
- Kısmi ödeme → Session status: `payment_pending`
- Ödeme başarısız → Hata mesajı, tekrar deneme seçeneği

## API Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/v1/sessions/{id}/bill` | Hesap özeti |
| POST | `/api/v1/sessions/{id}/payments` | Ödeme başlat |
| POST | `/api/v1/payments/{id}/split` | Hesap bölme oluştur |
| GET | `/api/v1/payments/{id}` | Ödeme durumu |
| POST | `/api/v1/payments/callback` | iyzico webhook |

## Ekranlar

### Müşteri Tarafı
1. **Hesap Özeti** — Tüm sipariş kalemleri, toplam
2. **Bölme Seçeneği** — 4 seçenek kartları
3. **Eşit Bölme** — Kişi sayısı, kişi başı tutar
4. **Ürün Bazlı Bölme** — Ürün seçim listesi
5. **Ödeme Formu** — Kart bilgileri (iyzico form)
6. **3D Secure** — Banka doğrulama sayfası
7. **Ödeme Sonucu** — Başarılı/başarısız ekranı

### Restoran Tarafı
1. **Ödeme Bildirimi** — Masa X ödeme yaptı
2. **Ödeme Geçmişi** — Günlük ödeme listesi
