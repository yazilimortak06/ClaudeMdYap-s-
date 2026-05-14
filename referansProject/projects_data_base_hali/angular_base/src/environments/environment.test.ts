// Kaynak: E:\Projeler\Angular\EvTechPanelAltunkaya\src\environments\environment.test.ts
// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {

  production: true,
  appVersion: 'v717demo1',
  isTest: false,
  // apiUrl: 'http://[TEST_HOST]:8000/web/',
  // apiUrl: 'https://[TEST_HOST]:443/web/',
  apiUrl: 'http://[TEST_HOST]:8001/v1/',
  tockenUrl: 'http://[TEST_HOST]:8097/v1/',
  fileApiUrl: 'http://[TEST_HOST]:8099/v1/',
  testApiUrl: 'http://[TEST_HOST]:54123/v1/',
  imageUrl: 'http://[TEST_HOST]:8099/v1/FileView/GetFile?code=',
  logApiUrl: 'http://[TEST_HOST]:8004/v1/',
  notificationApiUrl: 'http://[TEST_HOST]:8057/',
  stationApiUrl: 'http://[TEST_HOST]:8095/v1/',
  ocppApiUrl: 'http://[TEST_HOST]:8093/v1/',
  stationNotificationApiUrl: 'http://[TEST_HOST]:14258/'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
