// environment.template.ts
// Bu dosya template'tir. Gerçek değerleri environment.ts'e kopyalayıp doldurun.

export const environment = {
  production: false,

  // Backend API
  apiUrl: 'YOUR_API_BASE_URL',

  // SignalR hub endpoint
  signalrHubUrl: 'YOUR_SIGNALR_HUB_URL',

  // Uygulama ayarları
  defaultLanguage: 'tr',
  availableLanguages: ['tr', 'en'],

  // QR kod base URL
  qrBaseUrl: 'YOUR_QR_BASE_URL',

  // Harita API (varsa)
  mapApiKey: 'YOUR_MAP_API_KEY'
};
