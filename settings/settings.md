# Settings

## Kullanıcı Dosyası Şeması

Her kullanıcı için `settings/users/<isim>.md` adında bir dosya bulunur.
Kullanıcı adı kişi + makine kombinasyonundan oluşur (örn: `saidMasaustu`, `aliLaptop`).
Bu sayede aynı kişinin birden fazla bilgisayarı farklı dosyalarla temsil edilebilir.

### Örnek kullanıcı dosyası (`saidMasaustu.md`)

```markdown
# saidMasaustu

- **Kullanıcı adı:** saidMasaustu

## Proje Pathler

| Proje | Path |
|-------|------|
| RestaurantSystemBackend | C:\Projects\RestaurantSystemBackend |
| RestaurantSystemPanel   | C:\Projects\RestaurantSystemPanel   |
| RestaurantSystemQr      | C:\Projects\RestaurantSystemQr      |
```

## Kurallar

1. Kullanıcı dosyasındaki projeler `settings/projects.md` ile uyumlu olmalıdır.
2. Kullanıcı çalışmak istediği projeyi seçer. Seçilen proje `settings/projects.md` de yoksa önce oraya eklenir, sonra devam edilir.
3. Kullanıcı dosyasında projenin local pathi tanımlı değilse, kullanıcıdan path alınır ve dosyaya kaydedilir.
4. Tüm proje isimleri `settings/projects.md` den gelir — bu tek kaynak of truth'tur.
