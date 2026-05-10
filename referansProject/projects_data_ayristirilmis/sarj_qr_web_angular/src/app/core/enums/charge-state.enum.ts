// Kaynak: E:\Projeler\Angular\rotawattqrweb-master\rotawattqrweb-master\src\app\core\enums\charge-state.enum.ts
// ============================================================
// Sarj islemi durumu (OCPP tarafindaki state)
// activeProcessingSession.chargeState
// ============================================================
export enum ChargeState {
  ProcessStarting = 1,
  ProcessStart = 2,
  ProcessEnding = 3,
  PaymentBeingReceived = 4,
  PaymentFail = 5,
  Completed = 6,
  Failed = 7,
  Calculating = 8,
}
