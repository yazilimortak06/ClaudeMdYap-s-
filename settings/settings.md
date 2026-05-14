# Settings

## Kullanıcı Tipleri

| Tip | Açıklama |
|-----|----------|
| `normal` | İsim + proje bazlı rol sahibi kullanıcı |
| `yonetici` | Rol sorusu yoktur, direkt proje seçer |

---

## Roller (normal tip kullanıcılar için)

Bir kişi farklı projeler için farklı rollere sahip olabilir — **proje bazlı rol** (kartezyen çarpım: kişi × proje × rol).

| Rol | Açıklama | Çalışma Klasörü |
|-----|----------|-----------------|
| `gelistirici` | Kod yazar | `current_md/<proje>/<isim>/` |
| `tester` | Test yazar, bug raporlar | `current_md/<proje>/tester/` |
| `analiz_uzmani` | Derin analiz, gereksinim | `current_md/<proje>/is_analisti/` |
| `task_uzmani` | Task oluşturur, backlog yönetir | `current_md/<proje>/task_uzmani/` + `tasks/<proje>/` |
| `tasarimci` | UI/UX tasarımı | `current_md/<proje>/tasarimci/` |
| `arge_muhendisi` | Araştırma, prototip, teknoloji değerlendirme | `current_md/<proje>/arge_muhendisi/` |
| `yazilim_mimari` | Mimari tasarım, ADR, entegrasyon desenleri | `current_md/<proje>/yazilim_mimari/` |
| `kalite_muhendisi` | Kalite standartları, metrikler, teknik borç | `current_md/<proje>/kalite_muhendisi/` |

> `gelistirici` rolü için proje local path'i sorulur; diğer roller için sorulmaz.
> Rol bilgisi kullanıcı dosyasının `Projeler` tablosuna kaydedilir.

---

## Kullanıcı Dosyası Şeması

### Normal kullanıcı (`ali.md`)
```markdown
# ali

- **İsim:** ali
- **Tip:** normal

## Projeler
| Proje | Rol |
|-------|-----|
| RestaurantSystemBackend | gelistirici |
| RestaurantSystemPanel | task_uzmani |
| RestaurantSystemQr | - |

## Bilgisayar: masaustu
| Proje | Path |
|-------|------|
| RestaurantSystemBackend | C:\Projects\RestaurantSystemBackend |
| RestaurantSystemPanel | - |
| RestaurantSystemQr | - |

## Bilgisayar: laptop
| Proje | Path |
|-------|------|
| RestaurantSystemBackend | D:\dev\RestaurantSystemBackend |
| RestaurantSystemPanel | - |
| RestaurantSystemQr | - |
```

Roller bilgisayardan bağımsız (Projeler tablosunda), path'ler bilgisayar bazlı (`## Bilgisayar: <ad>` section'larında).
Yeni bilgisayardan ilk girişte `## Bilgisayar: <ad>` section'u oluşturulur, path'ler girilir.

### Ortak kullanıcı (`ortak.md`)
Birden fazla kişi kullanır. Her kişi kendi section'ında, aynı şemayı takip eder.

```markdown
# ortak

- **İsim:** ortak
- **Tip:** normal

## ali

### Projeler
| Proje | Rol |
|-------|-----|
| RestaurantSystemBackend | gelistirici |

### Bilgisayar: masaustu
| Proje | Path |
|-------|------|
| RestaurantSystemBackend | C:\Projects\... |

## muhammed_ali

### Projeler
| Proje | Rol |
|-------|-----|
| RestaurantSystemBackend | gelistirici |

### Bilgisayar: masaustu
| Proje | Path |
|-------|------|
| RestaurantSystemBackend | C:\Projects\... |
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
3. `gelistirici` rolü için proje local path'i sorulur; diğer roller için sorulmaz
4. Path'ler bilgisayar bazlıdır — `## Bilgisayar: <ad>` section'ına kaydedilir
5. Yeni bilgisayardan ilk girişte section yoksa oluşturulur, path'ler doldurulur
6. Tüm proje isimleri `settings/projects.md` den gelir — tek kaynak
5. `proje: tüm projeler` seçilince cross-project `_ilerleme/` görünümü açılır
6. Bir kişi farklı projeler için farklı rollere sahip olabilir (proje bazlı rol)
7. Rol sorusu Adım 3'te gelir, proje seçiminden SONRA
