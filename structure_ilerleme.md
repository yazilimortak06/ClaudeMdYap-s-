# Yapı İlerleme Notları

Bu dosya uzun oturum boyunca kaldığı yeri takip etmek için kullanılır.
Context büyüdüğünde buradan devam edilir.

---

## Oturum Başlangıcı: 2026-05-11

### Genel Hedef
Tüm referans projeleri gerçek kaynak koddan didik didik et:
- Base projeler kopyalanıp kullanılabilir hale gelsin
- Analiz dosyaları kitap kalitesinde olsun (tam kod blokları)
- projects_data_ayristirilmis tüm pattern'leri içersin

---

## Kaynak Proje Pathler

| Proje | Path |
|-------|------|
| sarj_backend_dotnet | E:\Projeler\Backend\RotaWattBackEnd |
| sarj_pro_backend_dotnet | E:\Projeler\Backend\SarjAllPro |
| sarj_vm_backend_dotnet | E:\Projeler\Backend\rotawattvmbackend-develop (1) |
| crm_backend | E:\Projeler\Backend\PixdinnCrm |
| perde_iot_backend | E:\Projeler\Backend\PixdinnPerdeci |
| qr_menu_mvc | E:\Projeler\Backend\QrMenu |
| sarj_ev_panel_angular | E:\Projeler\Angular\EvTechPanelAltunkaya |
| sarj_pro_panel_angular | E:\Projeler\Angular\SarjAllProPanel |
| sarj_qr_web_angular | E:\Projeler\Angular\rotawattqrweb-master |
| dijital_menu_angular | E:\Projeler\Angular\PixdinnNewMenu |
| yonetim_panel_angular | E:\Projeler\Angular\PixdinnYonetimPanel |
| RestaurantSystemPanel | E:\Projeler\Angular\RestaurantSystemPanel |
| RestaurantSystemQr | E:\Projeler\Angular\RestaurantSystemQr |

---

## İlerleme Durumu

### angular_base (projects_data_base_hali/angular_base)
- [x] Tur 1: app.module, routing, environments, tsconfig (EvTechPanelAltunkaya'dan)
- [ ] **Tur 2 AKTİF**: Tüm core/, shared_admin/, feature örneği — ajan a4ba4e754df49db43 çalışıyor
- [ ] Tur 3: Eksikleri tamamla

### dotnet_base (projects_data_base_hali/dotnet_base)
- [x] Tur 1: FrameworkCore (8 dosya), Log.Api, File.Api, Token.Api, docker-compose.base
- [ ] **Tur 2 AKTİF**: Bank.Api, Mobil.Api, Web.Api, Station.Api, Notification.Api, WorkerService, Gateway — ajan ab5e04d524dfe9747 çalışıyor
- [ ] Tur 3: Integration.Api, Ocpp.Api, MailSms.Api, tüm Application/Service örnekleri

### Analiz Dosyaları (referansProject/analiz/)

| Proje | Durum |
|-------|-------|
| sarj_backend_dotnet | Detaylı (PaymentController, BaseRepo, WrapperCore) — daha fazla API lazım |
| sarj_vm_backend_dotnet | Detaylı (VmConnectionService, OCPP) — iyi durumda |
| crm_backend | Orta — Infrastructure, tüm entities, tüm services lazım |
| qr_menu_mvc | Orta — Views, tam controller kodu lazım |
| sarj_ev_panel_angular | **ZAYIF** — AKTİF ajan aef02ad9daa23583f çalışıyor |
| sarj_pro_panel_angular | Orta |
| sarj_qr_web_angular | Orta |
| dijital_menu_angular | Zayıf |
| yonetim_panel_angular | Zayıf |
| restoran_panel_angular | Zayıf (scaffold) |
| perde_iot_backend | Orta |

### projects_data_ayristirilmis

| Proje | Durum |
|-------|-------|
| sarj_ev_panel_angular | Kısmi — dialog, chart, signalr, form patterns eksik |
| sarj_backend_dotnet | Kısmi — Mobil.Api controllers, Web.Api, Station.Api eksik |
| sarj_vm_backend_dotnet | İyi |
| crm_backend | Orta |
| qr_menu_mvc | İyi |
| perde_iot_backend | Orta |
| sarj_pro_panel_angular | Orta |
| sarj_qr_web_angular | Orta |
| dijital_menu_angular | Orta |
| yonetim_panel_angular | Orta |

---

## Commit Geçmişi Bu Oturumda

| Commit | İçerik |
|--------|--------|
| 7935997 | Domain rules, mimari gelisen, backend+angular rules |
| fbf210f | Tasks, agent_rules, tecrube, logs, ortak, shared |
| cbb1702 | İlk gerçek kod çıkarımı (base projeler + ayristirilmis tur 1) |
| (bekleniyor) | angular_base tam rewrite + dotnet_base tüm API'ler + analiz kitap kalitesi |

---

## Sonraki Yapılacaklar (Sıralı)

1. [ ] angular_base tur 2 tamamla (ajan bekle)
2. [ ] dotnet_base tur 2 tamamla (ajan bekle)  
3. [ ] sarj_ev_panel_angular analiz kitap kalitesi (ajan bekle)
4. [ ] sarj_backend_dotnet analiz — tüm API'ler (Mobil, Web, Station, Ocpp)
5. [ ] sarj_vm_backend_dotnet analiz — genişlet
6. [ ] crm_backend analiz — tam kod
7. [ ] qr_menu_mvc analiz — Views dahil tam kod
8. [ ] sarj_pro_panel_angular analiz — tam kod
9. [ ] sarj_qr_web_angular analiz — tam kod
10. [ ] dijital_menu_angular analiz — tam kod
11. [ ] yonetim_panel_angular analiz — tam kod
12. [ ] sarj_ev_panel_angular ayristirilmis — dialog, chart, form, signalr patterns
13. [ ] sarj_backend_dotnet ayristirilmis — Mobil.Api, Web.Api, Station.Api
14. [ ] dotnet_base tur 3 — Integration, Ocpp, MailSms API'leri
