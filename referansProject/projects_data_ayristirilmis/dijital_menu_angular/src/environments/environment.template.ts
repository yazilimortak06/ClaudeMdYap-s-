// environment.template.ts
// Bu dosya template'tir. Gerçek değerleri environment.ts'e kopyalayıp doldurun.
// environment.prod.ts production değerlerini içermelidir.

export const environment = {
  production: false,

  // emailJS entegrasyonu için
  emailjs: {
    serviceId: 'YOUR_EMAILJS_SERVICE_ID',
    templateId: 'YOUR_EMAILJS_TEMPLATE_ID',
    userId: 'YOUR_EMAILJS_USER_ID'
  },

  // Google reCAPTCHA
  recaptcha: {
    siteKey: 'YOUR_RECAPTCHA_SITE_KEY'
  },

  // API endpoint (varsa)
  apiUrl: 'YOUR_API_BASE_URL'
};
