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
| (bekle-son) | Tüm tur 2 işleri — EN SONA PUSH |

---

## Tamamlanan (Bu Oturum Tur 2)

- [x] sarj_backend_dotnet analiz → Mobil.Api, Web.Api, Station.Api, Ocpp.Api, Notification.Api, WorkerService, Gateway, Integration.Api (8 API + 8 kural)
- [x] sarj_vm_backend_dotnet analiz → tüm VmPanel controllers, entities, DbContext, Startup'lar
- [x] sarj_vm_backend_dotnet ayristirilmis → controllers_vmpanel, entities, startup (yeni dosyalar)
- [x] perde_iot_backend analiz → YENİDEN YAZILDI (iskelet proje gerçek durumu belgelendi)
- [x] perde_iot_backend ayristirilmis → startup_gercek.cs, controllers_gercek.cs
- [x] qr_menu_mvc analiz → YENİDEN YAZILDI (65+ entity, 7 controller, Redis, QR üretimi, Views)
- [x] qr_menu_mvc rules → 15 kural
- [x] crm_backend analiz → YENİDEN YAZILDI (13 entity, 7 service, 12 repo, 7 controller, 27 kural)
- [x] dijital_menu_angular analiz → YENİDEN YAZILDI (tam kod, emailJS, reCAPTCHA bug'ları)
- [x] yonetim_panel_angular analiz → YENİDEN YAZILDI (tam kod, 6 bug tespit)
- [x] sarj_ev_panel_angular ayristirilmis → dialog, form-add, form-edit, chart, signalr, pdf, file-upload, map, list, detail (10 pattern örneği + core/directives/pipes/base-datatable)
- [x] angular_base core/ modülü → adapters, bases, configs, date-core, directives, pipes, services, wrapper-core
- [x] dotnet_base Bank.Api, Mobil.Api, Station.Api, Notification.Api, Web.Api, GateWay.Api, WorkerService (88 dosya toplam)

## Limit Durumu
- Rate limit geldi: 04:10 Istanbul'da sıfırlanıyor
- Limit sonrası devam edilecek

## Kalan Yapılacaklar (Limit Sonrası)

### angular_base — shared_admin + feature şablonu EKSİK
- core/ eklendi ✓
- [ ] shared_admin/: layout.module, layout.component, aside, header, topbar, subheader, dialogs
- [ ] shared_admin/auth/: auth.guard, authentication-service
- [ ] shared_admin/utils/interseptors/: http interceptor
- [ ] shared_admin/general-material.module.ts
- [ ] shared_admin/template/error-pages/ (404, 403, 500)
- [ ] Feature şablon modülü (boş ama routing + module + component yapısı)
- [ ] Login sayfası

### sarj_ev_panel_angular analiz — EKSİK (ajan limit yedi)
- [ ] analiz.md: core/ tüm dosyalar tam kod, shared_admin/ tüm dosyalar, feature modülü tam kod
- [ ] rules.md: 20+ kural kod örnekleriyle

### sarj_qr_web_angular analiz — EKSİK
- [ ] analiz.md: models, enums, services, guards, modules tam kod
- [ ] rules.md

### sarj_pro_panel_angular analiz — EKSİK
- [ ] analiz.md + rules.md tam kod

### sarj_pro_backend_dotnet analiz — EKSİK
- [ ] analiz.md + rules.md tam kod

### dotnet_base tur 3 — EKSİK
- [ ] Integration.Api: Startup + controllers
- [ ] Ocpp.Api: Startup + WebSocket controller
- [ ] MailSms.Api: Startup + controller
- [ ] Application-Examples/: PaymentService, WalletService, consumer, Refit client
- [ ] Persistence-Examples/: Bank DbContext, repositories, EntityFluent
- [ ] Shared.Domain/ tam

### sarj_backend_dotnet ayristirilmis tur 3 — EKSİK
- [ ] controllers/mobil-api/ TÜM controller'lar
- [ ] controllers/web-api/ TÜM controller'lar
- [ ] controllers/station-api/ + ocpp-api/
- [ ] application/mobil-services/ + web-services/
