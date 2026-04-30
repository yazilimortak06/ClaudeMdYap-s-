# Agent Ana Kuralları

Bu dosyayı okuyan agent, tüm sistem genelinde nasıl davranacağını buradan öğrenir.
`main.md` akışını uygulamadan önce bu kuralları içselleştir.

## Kural Dosyaları

Her modun detaylı kuralları `rules/agent_rules/` altındaki ilgili dosyada tanımlanmıştır.
Moda geçmeden önce ilgili kural dosyasını oku.

| Mod / Konu | Dosya |
|------------|-------|
| Analiz | `rules/agent_rules/analiz_rules.md` |
| Geliştirme | `rules/agent_rules/gelistirme_rules.md` |
| İş Ekleme | `rules/agent_rules/is_ekleme_rules.md` |
| Domainsel Kural Tanımlama | `rules/agent_rules/domain_kural_tanimlama_rules.md` |
| Yeni Proje Ekleme | `rules/agent_rules/proje_ekleme_rules.md` |

## Yazılım Bilgi Dosyaları

Geliştirme yaparken kaliteli, güvenli, ölçeklenebilir kod için aşağıdaki rehberleri oku:

| Konu | Dosya |
|------|-------|
| Tasarım Desenleri | `rules/agent_rules/yazilim_bilgi/desenler.md` |
| Yazılım Prensipleri (SOLID, DRY, Clean Architecture) | `rules/agent_rules/yazilim_bilgi/prensipler.md` |
| Güvenlik (OWASP, Auth, Input Validation) | `rules/agent_rules/yazilim_bilgi/guvenlik.md` |
| Performans (N+1, Cache, Async) | `rules/agent_rules/yazilim_bilgi/performans.md` |
| Ölçeklenebilirlik | `rules/agent_rules/yazilim_bilgi/olceklenebilirlik.md` |
| Test Stratejileri | `rules/agent_rules/yazilim_bilgi/test_stratejileri.md` |

## Tecrübe Dosyaları

Aynı hataya düşmemek için geliştirme öncesi ilgili tecrübe dosyalarını oku:

| Kapsam | Dosya |
|--------|-------|
| Genel (proje bağımsız) | `rules/tecrube/ortak/genel.md` |
| RestaurantSystemBackend — Ortak | `rules/tecrube/RestaurantSystemBackend/ortak.md` |
| RestaurantSystemPanel — Ortak | `rules/tecrube/RestaurantSystemPanel/ortak.md` |
| RestaurantSystemQr — Ortak | `rules/tecrube/RestaurantSystemQr/ortak.md` |
| Kişi bazlı tecrübeler | `rules/tecrube/<proje>/<kullanıcı>.md` |

Hata yaşandığında veya exception alındığında:
1. İlgili proje ve kişi tecrübe dosyasına kaydet.
2. Genel bir ders çıkarıldıysa `ortak/genel.md` ye de ekle.

---

---

## 1. Genel Davranış

- `main.md` açıldığında adımları sırayla uygula: kullanıcı belirle → proje seç → mod seç.
- Her adımda kullanıcıya net seçenekler sun, seçimi bekle. Varsayımla ilerleme.
- Belirsiz bir durum varsa ilerlemeden önce sor.
- Uzun açıklamalar yapma. Kısa, net, madde madde ilet.
- Her önemli yapısal karar veya değişiklik `settings/structures/kararlar.md` e eklenir.

---

## 2. Dosya Okuma Sırası

Mod seçildiğinde aşağıdaki dosyaları sırasıyla oku, sonra çalışmaya başla:

| Mod | Okunacak Dosyalar (sırasıyla) |
|-----|-------------------------------|
| Geliştirme | `current_md/<proje>/agent.md` → `current_md/<proje>/mimari_gelisen.md` → `rules/proje_mimari_rules/<tip>.md` → `rules/proje_domain_rules/<proje>.md` |
| Analiz | `current_md/<proje>/<kullanıcı>/analiz.md` → `current_md/<proje>/ortak/analiz.md` → `rules/analiz_rules.md` |
| Domainsel kural tanımlama | `rules/proje_domain_rules/<proje>.md` → `current_md/<proje>/mimari_gelisen.md` |
| İş ekleme | `current_md/<proje>/<kullanıcı>/current.md` → `current_md/<proje>/ortak/ortak.md` |
| Task yönetimi | `tasks/<proje>/yapilacaklar.md` → `tasks/<proje>/havuz.md` → `tasks/<proje>/planlama.md` |

---

## 3. Dosya Yazma Kuralları

- **`current_md/<proje>/ortak/ortak.md`** — her commit/push sonrası güncellenir. Genel ilerleme ve son değişiklikler burada tutulur.
- **`current_md/<proje>/mimari_gelisen.md`** — önemli bir mimari karar alındığında güncellenir.
- **`current_md/<proje>/agent.md`** — aktif görev değiştiğinde güncellenir.
- **`current_md/<proje>/<kullanıcı>/current.md`** — kullanıcı iş değiştirdiğinde güncellenir.
- **`settings/structures/kararlar.md`** — yapısal bir karar alındığında güncellenir.
- **`rules/proje_domain_rules/<proje>.md`** — domain kuralı tanımlandığında veya güncellendiğinde yazılır.

---

## 4. Task Yönetimi

Her proje için `tasks/<proje>/` altında 3 dosya vardır:

- **`planlama.md`** — tartışmalar, kesinleşmemiş görevler, fikirler
- **`havuz.md`** — kesinleşmiş ama henüz alınmamış görevler (backlog)
- **`yapilacaklar.md`** — aktif olarak yapılacak / yapılan işler

**Akış:**
1. Yeni bir iş fikri veya tartışma → `planlama.md` ye ekle
2. Tartışma tamamlanıp kesinleşirse → `havuz.md` ye taşı, `planlama.md` den kaldır
3. İş alınırsa → `yapilacaklar.md` ye taşı, `havuz.md` den kaldır
4. İş tamamlanırsa → `yapilacaklar.md` den kaldır (veya tamamlandı olarak işaretle)

Taşıma işlemlerini kullanıcı onayı olmadan yapma.

---

## 5. Analiz Akışı

1. Analiz çalışması `current_md/<proje>/<kullanıcı>/analiz.md` üzerinde yapılır. Bu dosya kişiseldir, taslaktır, bağlayıcı değildir.
2. Analiz tamamlandığında kullanıcıya sor: "Bu analizi kesinleştirip ortak analize ekleyelim mi?"
3. Onay gelirse → `current_md/<proje>/ortak/analiz.md` ye tarih ve başlık ile ekle.
4. `ortak/analiz.md` deki her madde tüm ekip için bağlayıcıdır.
5. Kesinleşmiş bir analizi geri almak veya değiştirmek için ayrı bir tartışma açılması gerekir.

---

## 6. Kesinleşme Kuralları

- Hiçbir şeyi kullanıcı onayı olmadan kesinleştirme.
- Kesinleşme şu dosyaları etkiler: `ortak/analiz.md`, `havuz.md`, `mimari_gelisen.md`, `kararlar.md`
- Kesinleşen bir karar geri alınmak istenirse, önce kullanıcıyla tartış, sonra ilgili dosyayı güncelle.

---

## 7. Yasaklar

- **`current_md/<proje>/ortak/analiz.md`** doğrudan düzenlenmez. Sadece kesinleşme süreci üzerinden güncellenir.
- **`rules/proje_mimari_rules/<tip>.md`** dosyaları normal geliştirme akışında değiştirilemez. Değiştirmek için ayrı ve özel bir sohbet gereklidir.
- Kullanıcı onayı olmadan hiçbir dosyayı kesinleşmiş statüye taşıma.
- `settings/projects.md` dışından proje adı türetme — tek kaynak bu dosyadır.
- Kullanıcı dosyasında (`settings/users/<isim>.md`) olmayan bir proje path'i varsayımla kullanma.
