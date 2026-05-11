# Settings

## Kullanıcı Dosyası Şeması

Her kullanıcı için `settings/users/<isim>.md` adında bir dosya bulunur.
Kullanıcı adı kişi + makine kombinasyonundan oluşur (örn: `saidMasaustu`, `aliLaptop`).
Bu sayede aynı kişinin birden fazla bilgisayarı farklı dosyalarla temsil edilebilir.

## Kullanıcı Tipleri

| Tip | Açıklama | Ana Klasör |
|-----|----------|------------|
| `gelistirici` | Kod yazar, geliştirme yapar | `current_md/<proje>/<kullaniciAdi>/` |
| `is_analisti` | Derin analiz, task oluşturma, tartışma — kod yazmaz | `current_md/<proje>/is_analisti/` |
| `yonetici` | İlerleme takibi, iş atama, raporlama — kod yazmaz | `current_md/<proje>/yonetici/` |
| `tester` | Test senaryoları, bug raporlama, regresyon — kod yazmaz | `current_md/<proje>/tester/` |

> `is_analisti`, `yonetici`, `tester` özel kullanıcı tipleridir — proje başına bir tane, kişiye özgü değil.
> `gelistirici` tipi kişi bazlıdır (ali, muhammed_ali, said...).

---

### Örnek kullanıcı dosyası — geliştirici (`saidMasaustu.md`)

```markdown
# saidMasaustu

- **Kullanıcı adı:** saidMasaustu
- **Tip:** gelistirici

## Proje Pathler

| Proje | Path |
|-------|------|
| RestaurantSystemBackend | C:\Projects\RestaurantSystemBackend |
| RestaurantSystemPanel   | C:\Projects\RestaurantSystemPanel   |
| RestaurantSystemQr      | C:\Projects\RestaurantSystemQr      |
```

### Örnek kullanıcı dosyası — özel tip (`is_analisti.md`)

```markdown
# is_analisti

- **Kullanıcı adı:** is_analisti
- **Tip:** is_analisti

## Proje Pathler

| Proje | Path |
|-------|------|
| RestaurantSystemBackend | - |
| RestaurantSystemPanel   | - |
| RestaurantSystemQr      | - |
```

## Kurallar

1. Kullanıcı dosyasındaki projeler `settings/projects.md` ile uyumlu olmalıdır.
2. Kullanıcı çalışmak istediği projeyi seçer. Seçilen proje `settings/projects.md` de yoksa önce oraya eklenir, sonra devam edilir.
3. Kullanıcı dosyasında projenin local pathi tanımlı değilse, kullanıcıdan path alınır ve dosyaya kaydedilir.
4. Tüm proje isimleri `settings/projects.md` den gelir — bu tek kaynak of truth'tur.
