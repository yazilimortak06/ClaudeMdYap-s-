# claude_context

Her Claude terminali kendi oturum klasörünü burada açar.
Aynı kişi aynı anda birden fazla terminal açıp farklı proje ve roller ile çalışabilir.
Her klasör o terminale aittir — terminal kapanınca pasif kalır.

---

## Klasör Adlandırma

```
YYYY-MM-DD_HHmm_<isim>_<ProjeKisa>_<rol>/
```

Örnekler:
```
2026-05-13_1430_said_Panel_task_uzmani/
2026-05-13_1431_said_Backend_gelistirici/
2026-05-13_0900_ali_Qr_tasarimci/
```

> Aynı gün aynı kişi farklı terminallerde farklı rol ve proje ile çalışabilir.
> Saat ve dakika eklenerek çakışma önlenir.

---

## Her Klasörün İçi

```
<oturum_klasoru>/
├── meta.md       # Kim, ne zaman, hangi proje, hangi rol
├── oturum.md     # Bu oturumda ne yapıldı (agent sürekli günceller)
└── durum.md      # Şu an nerede — aktif görev, açık noktalar, bir sonraki adım
```

---

## meta.md Şablonu

```markdown
# Oturum Meta

- **Kişi:** <isim>
- **Proje:** <ProjeAdı>
- **Rol:** <rol>
- **Başlangıç:** YYYY-MM-DD HH:mm
- **Durum:** Aktif / Kapalı
```

---

## oturum.md Şablonu

```markdown
# Oturum Notları

## Yapılanlar
- [ ] -

## Alınan Kararlar
-

## Açık Kalan Konular
-
```

---

## durum.md Şablonu

```markdown
# Mevcut Durum

## Aktif Görev
-

## Çalışılan Modül / Dosya
-

## Bir Sonraki Adım
-

## Takılınan Noktalar
-
```

---

## Temizlik Politikası

- Kapalı oturumlar silinmez (geçmiş için faydalıdır).
- 30 günden eski kapalı oturumlar commit'te tutulur ama `eski/` alt klasörüne taşınabilir.
- Agent her zaman EN SON kendi klasörüne yazar; eski klasörleri okumaz.

---

## Agent Notu

> Bu klasör "ben kimim bu terminalde?" sorusunu yanıtlar.
> Aynı kişi iki farklı terminalde iki farklı iş yapıyorsa, her terminal kendi klasörünü açar.
> Ana kullanıcı dosyası (`settings/users/<isim>.md`) uzun vadeli bilgileri tutar;
> `claude_context/` ise o terminale özel, geçici ama yararlı hafıza alanıdır.
