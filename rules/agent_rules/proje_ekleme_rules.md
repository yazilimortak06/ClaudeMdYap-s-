# Proje Ekleme Kuralları

Yeni bir proje eklenirken aşağıdaki adımlar sırayla uygulanır.

---

## Adım 1 — Temel Bilgileri Al

Kullanıcıdan şunları sor:
- Proje adı
- Proje tipi (backend / frontend / mobile / other)
- Bağlı proje var mı? (opsiyonel — hangi projeyle konuşacak?)
- Referans alacağı projeler var mı? (`referansProject/referans_projects.md` listesinden)

---

## Adım 2 — `settings/projects.md` Güncelle

Tabloya yeni satır ekle:

```
| <ProjeAdı> | <tip> | <BağlıProje veya -> |
```

---

## Adım 3 — `current_md/` Altına Klasör Yap

`current_md/<ProjeAdı>/` oluştur ve içine:

- `agent.md` — agent bağlamı, aktif görev, shared referansları
- `mimari_gelisen.md` — proje ilerledikçe alınan kararlarla büyüyen mimari döküman
- `ortak/ortak.md` — her commit/push sonrası güncellenen ortak durum
- Her kullanıcı için `<kullanıcı>/current.md` — çalışılan modül, ne yapacak, ilerleme

Kullanıcı listesi için `settings/users/` klasörüne bak.

---

## Adım 4 — `rules/proje_domain_rules/` Altına Domain Kuralları Dosyası

`rules/proje_domain_rules/<ProjeAdı>.md` oluştur.

---

## Adım 5 — Bağlı Proje Varsa Shared Klasör Oluştur

`shared/<ProjeA>--<ProjeB>/shared.md` oluştur.

Her iki projenin `agent.md` dosyasına shared klasör referansını ekle.

---

## Adım 6 — Referans Proje Varsa

`referansProject/referans_projects.md` listesinde yoksa önce oraya ekle.

Referans eklerken **bir kereliğine** kullanıcıyla şu konuları tartış:
- Referans projeden ne alınacak? (mimari, klasör yapısı, kod stili, hepsi mi?)
- Referansla çelişen mevcut kurallar var mı? Hangisi öncelikli?
- Referansın hangi katmanları baz alınacak? (tümü mü, sadece belirli modüller mi?)

Bu tartışmanın kararlarını `referansProject/<ReferansProjeAdı>/` altında bir `notlar.md` dosyasına kaydet.

---

## Adım 7 — Mimari Tip Kontrolü

Proje tipi `rules/proje_mimari_rules/` altında tanımlı mı kontrol et.
- Tanımlıysa ilgili dosyayı agent bağlamına ekle.
- Tanımlı değilse yeni bir `<tip>.md` dosyası oluştur ve kullanıcıdan kuralları iste.

---

## Adım 8 — `tasks/` Altına Klasör Yap

`tasks/<ProjeAdı>/` oluştur ve içine:
- `planlama.md`
- `havuz.md`
- `yapilacaklar.md`

---

## Özet Kontrol Listesi

- [ ] `settings/projects.md` güncellendi
- [ ] `current_md/<ProjeAdı>/agent.md` oluşturuldu
- [ ] `current_md/<ProjeAdı>/mimari_gelisen.md` oluşturuldu
- [ ] `current_md/<ProjeAdı>/ortak/ortak.md` oluşturuldu
- [ ] Her kullanıcı için `current_md/<ProjeAdı>/<kullanıcı>/current.md` oluşturuldu
- [ ] `rules/proje_domain_rules/<ProjeAdı>.md` oluşturuldu
- [ ] Bağlı proje varsa `shared/` klasörü ve her iki `agent.md` güncellendi
- [ ] Referans proje varsa tartışma yapıldı ve kararlar kaydedildi
- [ ] Mimari tip dosyası kontrol edildi
- [ ] `tasks/<ProjeAdı>/` klasörü oluşturuldu
