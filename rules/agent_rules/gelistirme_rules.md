# Geliştirme Kuralları

---

## 1. Başlamadan Önce

Geliştirmeye başlamadan önce şu dosyaları sırasıyla oku:

1. `agent.md` — Aktif görev nedir, bağlam nedir?
2. `current_md/<proje>/mimari_gelisen.md` — Mimari kararlar ve geçmiş değişiklikler
3. `rules/proje_mimari_rules/<tip>.md` — Proje tipine uygun mimari kurallar (backend / angular)
4. `rules/proje_domain_rules/<proje>.md` — Projeye özgü domain kuralları

Tecrübe dosyalarını oku:
- `rules/tecrube/ortak.md` — Genel tecrübeler
- `rules/tecrube/<proje>.md` — Projeye özgü tecrübeler
- `rules/tecrube/<kullanici>.md` — Kullanıcıya özgü tercihler

Aktif görev `agent.md`'de kayıtlı değilse kullanıcıdan al ve kaydet.

---

## 2. Backend (.NET) Geliştirme Akışı

Her feature için sırayla ilerle:

**Adım 1 — Domain:**
- Entity sınıfını `Domain/Entities/` altına yaz
- Value object veya domain event gerekiyorsa ekle
- `BaseEntity`'den türet, soft-delete alanlarını dahil et

**Adım 2 — Persistence:**
- Entity configuration'ı `Persistence/Configurations/` altına yaz
- Migration oluştur: `dotnet ef migrations add <İsim>`
- Global query filter kontrolü: tenant_id ve IsDeleted filter'ları aktif mi?

**Adım 3 — Application:**
- `IRepository<T>` veya özel repository interface tanımla
- Service sınıfını `Application/Services/` altına yaz
- FluentValidation ile validator ekle
- `AutoMapper` profili oluştur veya mevcut profile'a mapping ekle

**Adım 4 — API:**
- Controller'ı `API/Controllers/` altına yaz
- DTO'ları `Application/DTOs/` altına yaz (Request / Response ayrımı)
- Swagger XML comment ekle
- `[Authorize]` ve `[RequiredRole]` attribute'larını kontrol et

Her adımda referans: `rules/proje_mimari_rules/backend.md`

---

## 3. Frontend (Angular) Geliştirme Akışı

Her feature için sırayla ilerle:

**Adım 1 — Model:**
- TypeScript interface'i `core/models/` altına yaz
- Backend DTO ile birebir eşleşmeli
- `enum` kullanılacaksa `core/enums/` altına ekle

**Adım 2 — Service:**
- Angular service'i `core/services/` altına yaz
- `HttpClient` ile API çağrıları (typed response)
- Observable döndür, subscribe etme — component'e bırak
- `takeUntilDestroyed` veya `takeUntil(destroy$)` ile memory leak önle

**Adım 3 — Component:**
- Template: Metronic class'larına uygun HTML, hardcoded string yok (`TranslatePipe`)
- Logic: `OnPush` change detection, typed `FormGroup`, `any` tipi yasak
- Smart/Dumb component ayrımı: veri çeken component ayrı, sadece gösteren ayrı

**Adım 4 — Route kaydı:**
- Lazy loading ile module veya standalone component'i route'a ekle
- `AuthGuard` ve `RoleGuard` uygulandı mı kontrol et
- Breadcrumb için route `data` alanını doldur

Her adımda referans: `rules/proje_mimari_rules/angular.md`

---

## 4. Kod Kalitesi

**Backend (.NET):**
- SOLID prensiplerine uy; özellikle Single Responsibility ve Dependency Inversion
- Tüm input validasyonu FluentValidation ile yapılır, controller'da `ModelState` kontrolü yok
- Mapping için her zaman AutoMapper kullan, manuel mapping yasak
- Loglama: `ILogger<T>` ile yapılandırılmış loglama, hassas veri loglanmaz
- Unit test: Service katmanı için xUnit + Moq, kritik business logic %80 coverage hedefi

**Frontend (Angular):**
- `ChangeDetectionStrategy.OnPush` tüm component'lerde zorunlu
- `takeUntilDestroyed` veya `takeUntil(destroy$)` tüm subscription'larda zorunlu
- Typed reactive forms (`FormGroup<T>`), `any` tipi kesinlikle yasak
- `console.log` production'a geçmez, loglama servisi kullan

**Referans projeler için bkz:**
- `referansProject/analiz/` — Referans proje analizleri
- `referansProject/projects_data_ayristirilmis/` — Ayrıştırılmış proje verileri

---

## 5. Tamamlama

Görev tamamlandığında şu adımları sırayla uygula:

1. **`agent.md`** — Aktif görevi "tamamlandı" olarak işaretle, bir sonraki görevi belirt
2. **`current_md/<proje>/mimari_gelisen.md`** — Önemli mimari karar alındıysa buraya ekle
3. **`rules/tecrube/`** — Hata yaşandıysa veya öğrenilen bir şey varsa tecrübe dosyasına ekle
4. **`current_md/<proje>/ortak/ortak.md`** — Ortak durumu güncelle (hangi modüller hazır, API endpoint'leri)
5. **`tasks/<proje>/yapilacaklar.md`** — Tamamlanan görevi kaldır
