# Menü Yönetimi

## Kullanıcı Hikayesi

> Bir restoran sahibi olarak, menümü dijital ortamda oluşturmak, kategorilere ayırmak, ürün eklemek/düzenlemek ve fiyatlandırmak istiyorum.

## Menü Hiyerarşisi

```
Menu (ör: "Ana Menü")
 └── Category (ör: "Başlangıçlar")
      └── MenuItem (ör: "Mercimek Çorbası")
           └── MenuItemOption (ör: "Büyük Boy +5₺")
```

## Özellikler

### Menü
- Şube bazlı menü tanımlama (her şubenin farklı menüsü olabilir)
- Birden fazla menü: "Ana Menü", "İçecek Menüsü", "Tatlı Menüsü"
- Aktif/pasif durumu, sıralama

### Kategori
- Menü altında kategori grupları
- Görsel ekleme, açıklama
- Sıralama (drag & drop desteği)

### Ürün (MenuItem)
- Ad, açıklama, fiyat, görsel
- Hazırlanma süresi (tahmini)
- Stok durumu (müsait/müsait değil) — anlık toggle
- Alerjen bilgileri (jsonb): gluten, süt, fıstık vb.
- Sıralama

### Ürün Seçenekleri (MenuItemOption)
- Grup bazlı seçenekler: "Boyut" (S/M/L), "Ekstra" (peynir, sos)
- Fiyat farkı (+/-)
- Zorunlu/opsiyonel
- Tekli/çoklu seçim (maxSelections)

## API Endpoint'leri

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/v1/branches/{id}/menus` | Menü listesi |
| POST | `/api/v1/menus` | Menü oluştur |
| PUT | `/api/v1/menus/{id}` | Menü güncelle |
| DELETE | `/api/v1/menus/{id}` | Menü sil |
| POST | `/api/v1/menus/{id}/categories` | Kategori ekle |
| PUT | `/api/v1/categories/{id}` | Kategori güncelle |
| DELETE | `/api/v1/categories/{id}` | Kategori sil |
| PUT | `/api/v1/categories/sort` | Kategori sıralama |
| POST | `/api/v1/categories/{id}/items` | Ürün ekle |
| PUT | `/api/v1/menu-items/{id}` | Ürün güncelle |
| DELETE | `/api/v1/menu-items/{id}` | Ürün sil |
| PUT | `/api/v1/menu-items/{id}/availability` | Stok durumu toggle |
| PUT | `/api/v1/menu-items/sort` | Ürün sıralama |
| POST | `/api/v1/menu-items/{id}/options` | Seçenek ekle |
| PUT | `/api/v1/menu-item-options/{id}` | Seçenek güncelle |
| DELETE | `/api/v1/menu-item-options/{id}` | Seçenek sil |

## Ekranlar

### Restoran Web/Mobil
1. **Menü Listesi** — Menülerin kartları
2. **Menü Düzenle** — Kategori listesi (drag & drop sıralama)
3. **Kategori Düzenle** — Ürün listesi
4. **Ürün Ekle/Düzenle** — Form: ad, açıklama, fiyat, görsel, alerjen, seçenekler
5. **Stok Yönetimi** — Hızlı toggle listesi (müsait/değil)
