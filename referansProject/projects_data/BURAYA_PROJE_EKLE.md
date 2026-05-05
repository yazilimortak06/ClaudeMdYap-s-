# Bu Klasöre Proje Ekle

Bu klasöre referans almak istediğin projelerin klasörlerini bırak.
Sonra agent'a söyle — otomatik olarak 3 çıktıyı oluşturur.

---

## Agent'a Söyleyeceğin Şey

> "projects_data klasörüne yeni proje(ler) ekledim, işle"

---

## Agent'ın Yapacakları

### 1. projects_data_ayristirilmis/<anonim_isim>/
Yapısal/core dosyalar — hassas bilgiler temizlenerek.
- Connection string, API key, şifre → `[PLACEHOLDER]`
- Şirket/marka adı → anonim isim

### 2. projects_data_base_hali/
Sadece 2 base proje var (angular_base, dotnet_base).
Yeni proje eklenince base'ler güncellenmez — yalnızca referans olarak analiz edilir.

### 3. referansProject/<anonim_isim>/analiz.md + rules.md
Detaylı analiz ve çıkarılan kurallar.

### 4. referans_projects.md
Master listeye eklenir.

---

## İsimlendirme Kuralı

Gerçek isimler → sektör+tip bazlı anonim:
- Pixdinn, RotaWatt, SarjAll → `sarj_backend_dotnet`, `sarj_ev_panel_angular`
- Arvento → `filo_projesi`
- Şirket adı içeren her isim anonimleştirilir

---

## Mevcut İsim Mapping

| Ham İsim | Anonim |
|---|---|
| EvTechPanelAltunkaya | sarj_ev_panel_angular |
| PixdinnCrm | crm_backend |
| PixdinnNewMenu | dijital_menu_angular |
| PixdinnPerdeci | perde_iot_backend |
| PixdinnYonetimPanel | yonetim_panel_angular |
| QrMenu | qr_menu_mvc |
| RestaurantSystemPanel | restoran_panel_angular |
| RotaWattBackEnd | sarj_backend_dotnet |
| rotawattqrweb-master | sarj_qr_web_angular |
| rotawattvmbackend-develop | sarj_vm_backend_dotnet |
| SarjAllPro | sarj_pro_backend_dotnet |
| SarjAllProPanel | sarj_pro_panel_angular |
