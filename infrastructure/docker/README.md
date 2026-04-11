# QR Pay Check — Docker Ortamı

## Geliştirme Ortamı

Servisleri başlatmak için:
```bash
docker-compose -f docker-compose.dev.yml up -d
```

Servisleri durdurmak için:
```bash
docker-compose -f docker-compose.dev.yml down
```

Servis durumunu kontrol etmek için:
```bash
docker-compose -f docker-compose.dev.yml ps
```

## Servisler

| Servis | Port | Kullanıcı | Şifre |
|--------|------|-----------|-------|
| PostgreSQL | 5432 | qrpaycheck | qrpaycheck_dev |
| Keycloak | 8080 | admin | admin |

### PostgreSQL Bağlantı Bilgisi
```
Host=localhost;Port=5432;Database=qrpaycheck;Username=qrpaycheck;Password=qrpaycheck_dev
```

### Keycloak Admin Console
http://localhost:8080 → Kullanıcı: `admin` / Şifre: `admin`
