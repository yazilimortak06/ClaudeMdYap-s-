// KAYNAK (Listeners): E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Core\Applications\Vm.Application\Services\ConnectionManagement\VmConnectionService.Listeners.cs
// KAYNAK (Reconnect): ...\VmConnectionService.Reconnect.cs
// VmConnectionService - PARTIAL: Server/CPO mesaj dinleme ve yeniden bağlanma

// ================================================================
// LISTENERS PARTIAL
// ================================================================
// ListenServerMessages: Server'dan gelen OCPP mesajları dinler
// ListenCpoMessages: CPO'dan gelen OCPP mesajları dinler
// Desteklenen Server/CPO → Device mesajları:
//   - Tip 3 (CALLRESULT): StartTransactionResponse, BootNotificationResponse, HeartBeatResponse, StopTransactionResponse, AuthorizeResponse
//   - Tip 2 (CALL): RemoteStartTransaction, RemoteStopTransaction, Reset, ChangeAvailability,
//                   TriggerMessage, ChangeConfiguration, CancelReservation, ClearCache,
//                   DataTransfer, GetConfiguration, GetDiagnostics, GetLocalListVersion,
//                   ReserveNow, SendLocalList, SetChargingProfile, UnlockConnector, UpdateFirmware

// Önemli pattern:
// - Her gelen mesaj deserialize edilerek türü anlaşılmaya çalışılır (try/catch ile)
// - PendingServerMessages / PendingCpoMessages ConcurrentDictionary ile timeout takibi
// - WebSocket kapatıldığında ServerDisconnected/CpoDisconnected çağrılır
// - finally blokunda WebSocket dispose edilir ve null'a çekilir → reconnect'in tetiklenmesi sağlanır

// ================================================================
// RECONNECT PARTIAL
// ================================================================
// CheckAndReconnectServer: Server bağlantısı kopuksa background reconnect başlatır
// CheckAndReconnectCpo: CPO bağlantısı kopuksa background reconnect başlatır
// EnsureServerListenTaskStarted: Server listen task'ı başlatır/kontrol eder
// EnsureCpoListenTaskStarted: CPO listen task'ı başlatır/kontrol eder
// ReconnectServerInBackground: Server WebSocket yeniden bağlantısını background'da yapar
// ReconnectCpoInBackground: CPO WebSocket yeniden bağlantısını background'da yapar

// Önemli pattern:
// - ConcurrentDictionary.TryAdd ile "zaten reconnect var mı?" kontrolü
// - Per-device SemaphoreSlim (ReconnectSemaphore) ile race condition önleme
// - Başarılı reconnect sonrası: DrainPendingMeterValues + TriggerMessage(BootNotification/StatusNotification)
// - Başarılı reconnect sonrası: ListenTask yeniden başlatılır

// ================================================================
// KEY ALGORITHM: Exponential Backoff Retry
// ================================================================
// - maxAttempts: 3 (reconnect), 5 (ilk kurulum)
// - delay: 1000ms başlar, Math.Min(delay*2, 8000) ile büyür
// - Her attempt'te yeni ClientWebSocket oluşturulur (eski dispose edilir)

// ================================================================
// KEY CLASSES: PendingOcppMessage, VmConnectionSessionDto key fields
// ================================================================
/*
class PendingOcppMessage:
    string MessageId
    string Action
    DateTime SentTime
    CancellationTokenSource TimeoutCts

class VmConnectionSessionDto key fields:
    string Identifier
    WebSocket DeviceWebSocket
    VmConnectionServerItemDto ServerItem
    VmConnectionCpoItemDto CpoItem
    CancellationTokenSource DeviceCancellationTokenSource
    CancellationTokenSource ServerCancellationTokenSource
    CancellationTokenSource CpoCancellationTokenSource
    ConcurrentDictionary<string, PendingOcppMessage> PendingServerMessages
    ConcurrentDictionary<string, PendingOcppMessage> PendingCpoMessages
    DateTime? LastHeartbeatTime
    int MissedHeartbeatCount
    int HeartbeatInterval
    bool IsBootNotificationAccepted
    TransactionTargetEnum TransactionTarget  (VM_ONLY / CPO / SERVER / BOTH)
    ConnectionStateEnum CurrentConnectionState
    DateTime StateEnteredTime
    DateTime? LastServerDisconnectTime
    DateTime? LastCpoDisconnectTime
    SemaphoreSlim HeartbeatMonitorSemaphore
    SemaphoreSlim ConnectionHealthMonitorSemaphore
    Task HeartbeatMonitorTask
    Task ConnectionHealthMonitorTask
    ConcurrentDictionary<string, DateTime> SentStartTransactions  (dedup)
    ConcurrentDictionary<string, DateTime> SentStopTransactions   (dedup)

class VmConnectionServerItemDto:
    ClientWebSocket ServerWebSocket
    CancellationTokenSource ServerCancellationTokenSource
    Task ListenTask
    SemaphoreSlim ListenTaskSemaphore
    SemaphoreSlim ReconnectSemaphore
    SemaphoreSlim SendSemaphore

class VmConnectionCpoItemDto:
    ClientWebSocket CpoWebSocket
    Task ListenTask
    SemaphoreSlim ListenTaskSemaphore
    SemaphoreSlim ReconnectSemaphore
    SemaphoreSlim SendSemaphore
*/
