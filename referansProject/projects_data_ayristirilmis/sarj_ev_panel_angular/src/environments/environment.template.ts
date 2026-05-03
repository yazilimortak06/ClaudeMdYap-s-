// environment.template.ts
// Bu dosya sadece key yapısını gösterir. Gerçek değerler environment.ts dosyalarında tutulur.
// Hassas bilgi (URL, token) bu dosyada yer almaz.

export const environment = {
  production: false,
  appVersion: '[APP_VERSION]',

  // Ana API endpoints
  apiUrl: '[API_URL]',
  webSiteApiUrl: '[WEBSITE_API_URL]',
  tockenUrl: '[TOKEN_URL]',

  // Dosya/medya servisleri
  fileApiUrl: '[FILE_API_URL]',
  imageUrl: '[IMAGE_URL]',

  // Özelleşmiş servisler
  logApiUrl: '[LOG_API_URL]',
  notificationApiUrl: '[NOTIFICATION_API_URL]',
  stationApiUrl: '[STATION_API_URL]',
  ocppApiUrl: '[OCPP_API_URL]',
};
