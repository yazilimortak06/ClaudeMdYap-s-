# Projects

| Proje | Tip | Bağlı Projeler |
|-------|-----|----------------|
| RestaurantSystemBackend | backend | - |
| RestaurantSystemPanel   | frontend | RestaurantSystemBackend |
| RestaurantSystemQr      | frontend | RestaurantSystemBackend |

## Bağlantı Kuralları

- "Bağlı Projeler" alanı opsiyoneldir. `-` ise bağımsız proje demektir.
- İki proje birbirine bağlıysa `shared/<ProjeA>--<ProjeB>/` klasörü oluşturulur.
- Bağlantılı projelerin agentları bu klasörü okuyup yazarak birbirini takip eder.
  - Örn: Backend agenı endpoint tanımını buraya yazar, Panel agenı buradan okur.
