# sarj_pro_panel_angular — Çıkarılan Kurallar

Bu dosya sarj_ev_panel_angular/rules.md üzerine inşa edilir.
Aşağıda yalnızca bu projeden öğrenilen ek veya farklı kurallar yer alır.
Temel kurallar için `sarj_ev_panel_angular/rules.md`'ye bakınız.

---

## 1. Model Katmanı Organizasyon Kuralları

**Kural:** Model sayısı 8'i aştığında model klasörlerini alt kategorilere ayır:

```
models/
├── authentication/    (kimlik modelleri)
├── authority/         (yetki modelleri)
├── media-file/        (dosya/medya modelleri)
├── location/          (coğrafi lokasyon)
├── admin-user/        (kullanıcı modelleri)
├── parameter/         (parametre modelleri)
├── parameter-group/
├── parameter-value/
├── policy/
├── api-exception/     (hata modelleri)
└── request-response/  (API sarmalayıcılar)
```

**Kural:** `request-response/` veya `api-exception/` gibi genel API modelleri her zaman model katmanında yer almalı. Bu modeller feature boyunca kullanılır, tekrara düşmez.

**Kural:** `requestResponse` modeli (veya generic api response wrapper), backend'den dönen standart response yapısını sarmalamak için kullan:
```typescript
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors?: string[];
}
```

---

## 2. Enum Katmanı Organizasyon Kuralları

**Kural:** Enum'ları kategorilere göre klasörle, tek düz dosya olarak bırakma:

```
enums/
├── user-type/
│   └── user-type.enum.ts
└── media-file/
    ├── file-extension.enum.ts
    ├── media-type.enum.ts
    ├── file-type.enum.ts
    └── image-type.enum.ts
```

**Kural:** Dosya/medya yönetimi içeren projelerde şu enum kategorileri gerekir:
- `file-extension` — izin verilen uzantılar (jpg, png, pdf, xlsx vs.)
- `media-type` — MIME tipleri
- `file-type` — business tiplerle (image, document, video)
- `image-type` — görsel kategoriler (thumbnail, cover, gallery vs.)

**Kural:** Enum değerlerini backend ile sync tut. Backend'de `enum` veya `const` kullanıyorsa Angular'da aynı değerleri kullan.

---

## 3. Feature-Specific Pipe Kuralı

**Kural:** Pipe yalnızca tek bir feature modülünde kullanılıyorsa, `core/pipes/`'a değil `feature/pipes/`'a taşı:

```
feature/
├── components/
├── models/
├── services/
├── pipes/     ← feature-specific pipe'lar burada
└── enums/
```

**Kural:** Pipe birden fazla feature veya core tarafından kullanılıyorsa `core/pipes/`'a taşı.

---

## 4. Infrastructure Panel Pattern

**Kural:** "Platform altyapısı" yönetim paneli (kullanıcı yönetimi, parametreler, politikalar, dil, log) için feature kategorileri:

```
feature/components/
├── auth/              — zorunlu
├── authority-management/   — zorunlu
├── admin-management/  — zorunlu
├── parameters/        — platform için temel
├── system-parameter/  — uygulama ayarları
├── language/          — i18n yönetimi
├── policy/            — kullanım şartları, KVKK
└── log/               — işlem geçmişi
```

Bu 8 kategorinin tamamı bir "platform admin panel"i için minimal ama yeterli set.

---

## 5. Fake Data Kararı

**Kural:** sarj_pro'da `_fake/` klasörü yok. Eğer proje başından API ile geliştirme yapılıyorsa, `_fake/` klasörü ekleme. Sadece UI-first veya backend hazır olmadan geliştirme yapılıyorsa ekle.

**Kural:** Fake data kullanıyorsan, `_fake/` klasörü `AppModule`'e koşullu olarak import edilmeli:
```typescript
// AppModule
imports: [
  environment.production ? [] : [FakeModule]
]
```

---

## 6. Feature Scope vs Model Scope Dengesi

**Kural:** Feature scope küçük, model scope büyük olabilir. Bu bir sorun değil. Eğer panel backend'in karmaşık domain modelini yönetiyorsa, model katmanı her zaman feature UI'den daha kapsamlı olur.

**Kural:** Model katmanının büyüklüğü proje karmaşıklığının göstergesidir. 13 model grubu, projenin ciddi bir backend'e bağlı olduğunu söyler.

**Kural:** Backend'i mirror'layan model yapısı kur — backend'deki her entity için Angular'da karşılık gelen bir model klasörü olsun.
