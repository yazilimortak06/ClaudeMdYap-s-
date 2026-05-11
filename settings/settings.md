# Settings

## Kullanıcı Tipleri

| Tip | Açıklama |
|-----|----------|
| `normal` | İsim + rol sahibi kullanıcı (geliştirici, tester, analiz_uzmani, task_uzmani, tasarimci) |
| `yonetici` | Rol sorusu yoktur, direkt proje seçer |

---

## Roller (normal tip kullanıcılar için)

| Rol | Açıklama | Çalışma Klasörü |
|-----|----------|-----------------|
| `gelistirici` | Kod yazar | `current_md/<proje>/<isim>/` |
| `tester` | Test yazar, bug raporlar | `current_md/<proje>/<isim>/` + `tester/public/` |
| `analiz_uzmani` | Derin analiz, gereksinim | `current_md/<proje>/<isim>/` + `is_analisti/public/` |
| `task_uzmani` | Task oluşturur, backlog yönetir | `current_md/<proje>/<isim>/` + `tasks/<proje>/` |
| `tasarimci` | UI/UX tasarımı | `current_md/<proje>/<isim>/` + `tasarimci/public/` |

---

## Kullanıcı Dosyası Şeması

### Normal kullanıcı (`ali.md`)
```markdown
# ali

- **İsim:** ali
- **Rol:** gelistirici

## Proje Pathler

| Proje | Path |
|-------|------|
| RestaurantSystemBackend | C:\Projects\RestaurantSystemBackend |
```

### Yönetici kullanıcı (`yoneticiSaid.md`)
```markdown
# yoneticiSaid

- **İsim:** yoneticiSaid
- **Tip:** yonetici
```

---

## Kurallar

1. Kullanıcı adı: isim veya isim+makine kombinasyonu (örn: `saidMasaustu`)
2. Yönetici tipinde `Rol` alanı yoktur, `Tip: yonetici` yeterlidir
3. `gelistirici` rolü için proje local pathi sorulur; diğer roller için sorulmaz
4. Tüm proje isimleri `settings/projects.md` den gelir — tek kaynak
5. `proje: tüm projeler` seçilince cross-project `_ilerleme/` görünümü açılır
