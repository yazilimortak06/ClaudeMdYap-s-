// KAYNAK (Monitoring): E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Core\Applications\Vm.Application\Services\ConnectionManagement\VmConnectionService.Monitoring.cs
// KAYNAK (WebSocketSend): ...\VmConnectionService.WebSocketSend.cs
// VmConnectionService - PARTIAL: Monitoring (heartbeat, health) ve WebSocket gönderim

// ================================================================
// MONITORING PARTIAL KEY METHODS
// ================================================================

// EnsureHeartbeatMonitorStarted:
// - Per-device HeartbeatMonitorSemaphore ile tek monitor başlatılır
// - TaskCompletionSource placeholder pattern (CompletedTask bug'ına karşı)
// - MonitorHeartbeatTimeout task'ı başlatılır

// EnsureConnectionHealthMonitorStarted:
// - Aynı pattern, ConnectionHealthMonitorSemaphore ile
// - MonitorConnectionHealth task'ı başlatılır

// MonitorHeartbeatTimeout:
// - Her 10 saniyede bir kontrol
// - BootNotification kabul edilmemişse skip
// - Timeout = HeartbeatInterval + max(30s, %50 interval) [CRITICAL FIX #26]
// - 3 ardışık miss → CancellationToken cancel + WebSocket Close
// - TTL cleanup: SentStartTransactions / SentStopTransactions (5dk üzeri temizlenir)

// MonitorConnectionHealth:
// - Her 30 saniyede çalışır (ConnectionMonitorIntervalMs)
// - Server bağlantısı kopuksa reconnect dener
// - CPO bağlantısı kopuksa reconnect dener
// - CPO WebSocket Open ama ListenTask ölüyse listener yeniden başlatılır [CRITICAL FIX #28]
// - DrainPendingStopTransactions + DrainPendingMeterValues çağrılır

// TryTransitionConnectionState:
// - TRY_ESTAB → CONNECTED: BootNotification kabul + 20s stabilite şartı
// - CONNECTED → TRY_ESTAB: 3 ardışık ping failure şartı
// - Debounce ile bağlantı flapping önlenir

// SendTriggerMessageToDevice:
// - [2, uuid, "TriggerMessage", {requestedMessage: "BootNotification"|"StatusNotification"}]
// - Reconnect sonrası cihazı yeniden kaydetmek için kullanılır

// ================================================================
// WEBSOCKETSEND PARTIAL KEY METHODS
// ================================================================

// SendDeviceMessageToCpo:
// - _vmSessionStatusDict'ten mevcut session alınır
// - CPO WebSocket kapalıysa ReconnectSemaphore ile thread-safe reconnect
// - SSL bypass config: Security:BypassCertificateValidation
// - Başarılı reconnect sonrası ListenCpoMessages başlatılır
// - Gönderim tracking: PendingCpoMessages'e eklenir, 30s timeout
// - SendSemaphore ile serialized send (concurrent send engellenir)
// - 30s CancelAfter ile send timeout

// SendDeviceMessageToServer:
// - Aynı pattern, Server için
// - ServerWebSocket kapalıysa ReconnectSemaphore ile reconnect
// - PendingServerMessages tracking
// - SendSemaphore ile serialized send

// SendMessageToDevice:
// - _vmSessionStatusDict'ten session alınır
// - DeviceWebSocket.State != Open ise exception
// - DeviceCancellationTokenSource ile linked 30s timeout
// - InvalidOperationException, WebSocketException, ObjectDisposedException handling

// ================================================================
// GÜVENLİK KODU ÖRÜNTÜLERİ
// ================================================================
/*
1. SSL validation:
   - Security:BypassCertificateValidation = false (default, prod)
   - Security:TrustedCertificateThumbprints[] (güvenilir sertifika parmakizi listesi)
   - true olursa tüm sertifikalar kabul edilir (dev/test için)

2. Thread safety:
   - ConcurrentDictionary (session dict)
   - SemaphoreSlim (reconnect, listenTask, send)
   - Per-device semaphore (global yerine)
   - Interlocked.CompareExchange (firstConnection flag)

3. Resource cleanup:
   - CancellationTokenSource.Cancel + Dispose (try/catch ile)
   - WebSocket.Dispose (finally blokunda)
   - MemoryStream.Dispose (using ile)
   - PendingMessage.TimeoutCts.Dispose (finally blokunda)
*/
