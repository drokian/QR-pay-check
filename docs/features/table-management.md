# Masa & QR Yönetimi

## Kullanıcı Hikayesi

> Bir restoran sahibi olarak, şubemdeki masaları tanımlamak, her masaya unique QR kod oluşturmak ve basılabilir formatta indirmek istiyorum.

## Özellikler

### Masa Yönetimi
- Masa ekleme: numara, kapasite, bölge (Bahçe, İç Mekan, Teras vb.)
- Masa düzenleme, silme (aktif oturum yoksa)
- Aktif/pasif durumu
- Masa durumu görüntüleme: Boş, Dolu, Ödeme Bekliyor

### QR Kod Yönetimi
- Her masa için unique QR kod oluşturma
- QR kod URL formatı: `https://app.qrpaycheck.com/qr/{unique-code}`
- QR kod görseli indirme (PNG, yüksek çözünürlük)
- Toplu QR kod indirme (ZIP — tüm masalar)
- QR kod yenileme (eskisini devre dışı bırakır)

### Masa Durumu Takibi (Realtime)
- Hangi masa dolu/boş → SignalR ile anlık güncelleme
- Masadaki aktif oturum süresi
- Masadaki toplam sipariş tutarı

## API Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/v1/branches/{id}/tables` | Masa listesi |
| POST | `/api/v1/branches/{id}/tables` | Masa ekle |
| PUT | `/api/v1/tables/{id}` | Masa güncelle |
| DELETE | `/api/v1/tables/{id}` | Masa sil |
| POST | `/api/v1/tables/{id}/generate-qr` | QR kod oluştur/yenile |
| GET | `/api/v1/tables/{id}/qr-code` | QR kod görseli indir |
| GET | `/api/v1/branches/{id}/tables/qr-codes` | Toplu QR indir (ZIP) |
| GET | `/api/v1/branches/{id}/tables/status` | Masa durumları (realtime) |

## Ekranlar

### Restoran Web/Mobil
1. **Masa Listesi** — Grid/liste görünümü, durum renkleri (yeşil: boş, kırmızı: dolu)
2. **Masa Ekle/Düzenle** — Form: numara, kapasite, bölge
3. **QR Kod Görüntüleme** — QR preview, indirme butonu
4. **Masa Detay** — Aktif oturum, siparişler, toplam tutar
