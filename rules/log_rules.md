# log_rules.md

Her agent, her oturum sonunda `logs/<ProjeAdı>/<isim>.md` dosyasına bir log girişi yazar.
Log, bir sonraki oturumda Claude'un bağlamı hızlıca kurabilmesi için tasarlanmıştır.

---

## Dosya Yolu

```
logs/<ProjeAdı>/<isim>.md
```

- `<ProjeAdı>`: çalışılan proje (örn: `RestaurantSystemBackend`)
- `<isim>`: kullanıcı adı (örn: `said`)
- Dosya yoksa oluştur. Her yeni giriş **en üste** eklenir.

---

## Log Girişi Formatı

```markdown
---
## [YYYY-MM-DD HH:mm] | <isim> | <ProjeAdı> | <rol>

**Özet:** Oturumda ne yapıldığını tek cümlede özetle.

### Yapılanlar
- (somut, tamamlanan işler — dosya adı / endpoint / bileşen gibi detayla)

### Alınan Kararlar
- (ne karar verildi + neden — kısa ama gerekçeli)

### Değişen Dosyalar
- path/to/file.cs — (ne değişti, tek satır)

### Yarım Kalanlar
- (bitmeden kalan işler — bir sonraki oturumun başlangıç noktası)

### Blokerlar
- (varsa: ne engel, kimin cevabı bekleniyor, yoksa "yok")

---
```

---

## Yazma Kuralları

1. **Her oturum sonu zorunludur.** Kısa da olsa yazılır, atlanmaz.
2. **Özet cümlesi en kritik kısım** — Claude bunu okuyarak oturumu anlar.
3. **Yapılanlar somut olacak:** "geliştirme yapıldı" değil, "OrderController POST /orders endpoint'i eklendi."
4. **Kararlar gerekçeli olacak:** "soft delete kullanıldı" değil, "soft delete kullanıldı — verinin geri yüklenmesi gereksinimi var."
5. **Yarım kalanlar açık uçlu değil, aksiyon bazlı:** "test yaz" değil, "OrderService edge case testleri yazılacak."
6. **Yeni giriş en üste eklenir** — Claude her zaman en son oturumu ilk okur.
7. **Worktree modunda:** Her `<rol>-<proje>/` klasörü kendi `ilerleme.md` ve `hafiza.md` dosyalarını günceller. Ek olarak bu log dosyasına da yazar.

---

## Oturum Başında Okuma

`main.md` Adım 4'te bağlam yüklenirken:

- `logs/<ProjeAdı>/<isim>.md` dosyasının **ilk log girişini** oku (en üstteki `---` bloğu).
- Özet + Yarım Kalanlar + Blokerlar kısmını bağlama ekle.
- Tüm dosyayı okuma, sadece son giriş yeterli.

---

## Worktree Koordinatör Logu

Koordinatör de her oturum sonunda kendi log girişini yazar:

```
logs/<ProjeAdı>/koordinator_<isim>.md
```

Format aynıdır. Ek olarak şu bölüm eklenir:

```markdown
### Agent Durumu
| Klasör | Son Durum | Yarım Kalan |
|--------|-----------|-------------|
| gelistirici-backend | ... | ... |
| gelistirici-panel | ... | ... |
```
