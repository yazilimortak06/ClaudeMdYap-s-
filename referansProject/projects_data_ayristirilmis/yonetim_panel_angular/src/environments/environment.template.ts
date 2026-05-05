// environment.template.ts
// Bu dosya sadece key yapısını gösterir. Gerçek değerler environment.ts dosyalarında tutulur.
// Hassas bilgi (URL, token) bu dosyada yer almaz.

export const environment = {
  production: false,
  appVersion: '[APP_VERSION]',

  // Ana API endpoint
  apiUrl: '[API_URL]',

  // Dosya/medya servisleri
  fileApiUrl: '[FILE_API_URL]',
  imageUrl: '[IMAGE_URL]',
};
