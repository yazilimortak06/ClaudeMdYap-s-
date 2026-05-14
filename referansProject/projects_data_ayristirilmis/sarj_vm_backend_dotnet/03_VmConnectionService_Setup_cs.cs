// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Core\Applications\Vm.Application\Services\ConnectionManagement\VmConnectionService.Setup.cs
// VmConnectionService - PARTIAL: Server ve CPO WebSocket baglanti kurulumu
// Ozellikler: SSL strict -> fallback bypass, exponential backoff retry

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Domain.Dto.VmDto.VmConnectionDtos;
using Shared.Domain.Dto.VmDto.VmCpoConnectionDtos;
using Shared.Domain.Dto.VmDto.VmServerConnectionDtos;
using Shared.Domain.ServiceInterfaces.VmServiceInterfaces.ConnectionManagementsServiceInterfaces;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Vm.Application.Services.ConnectionManagement
{
    public partial class VmConnectionService
    {
        /// <summary>
        /// Faz 1a: Server WebSocket baglantisinı kurar.
        /// </summary>
        private async Task<(Uri uriServer, bool hasValidServerUrl)> SetupServerConnection(
            VmConnectionSessionDto vmConnectionSession,
            string serverUrlAddress)
        {
            bool hasValidServerUrl = !string.IsNullOrWhiteSpace(serverUrlAddress);
            if (!hasValidServerUrl)
            {
                _logger.LogInformation($"Server URL not configured for device {vmConnectionSession.Identifier}. Skipping server connection.");
                return (null, false);
            }

            Uri uriServer = new Uri(serverUrlAddress + vmConnectionSession.Identifier);

            if (vmConnectionSession.ServerItem == null ||
                vmConnectionSession.ServerItem.ServerWebSocket == null ||
                (vmConnectionSession.ServerItem.ServerWebSocket.State != WebSocketState.Open &&
                 vmConnectionSession.ServerItem.ServerWebSocket.State != WebSocketState.Connecting))
            {
                try
                {
                    GetOrAddVmServerConnectionResponseDto? vmServerConnection = null;
                    GetOrAddVmServerConnectionRequestDto getOrAddVmServerConnectionRequest = new GetOrAddVmServerConnectionRequestDto();
                    getOrAddVmServerConnectionRequest.VmDeviceConnectionId = vmConnectionSession.VmDeviceConnectionId;
                    getOrAddVmServerConnectionRequest.Date = DateTime.Now;

                    using (var scope = _services.CreateScope())
                    {
                        IVmServerConnectionManagementService vmServerConnectionManagementService = scope.ServiceProvider.GetRequiredService<IVmServerConnectionManagementService>();
                        vmServerConnection = await vmServerConnectionManagementService.GetOrAddVmServerConnection(getOrAddVmServerConnectionRequest);
                    }

                    if (vmServerConnection != null)
                    {
                        _logger.LogInformation($"Attempting SERVER connection for device {vmConnectionSession.Identifier} to URL: {uriServer}");
                        try
                        {
                            vmConnectionSession.ServerCancellationTokenSource?.Cancel();
                            vmConnectionSession.ServerCancellationTokenSource?.Dispose();
                        }
                        catch (Exception disposeEx) { _logger.LogDebug(disposeEx, $"[SERVER SETUP] Non-critical dispose failed for {vmConnectionSession.Identifier}"); }
                        vmConnectionSession.ServerCancellationTokenSource = new CancellationTokenSource();
                        vmConnectionSession.ServerItem = new VmConnectionServerItemDto();
                        vmConnectionSession.ServerItem.ServerWebSocket = new ClientWebSocket();
                        vmConnectionSession.ServerItem.ServerWebSocket.Options.KeepAliveInterval = TimeSpan.Zero;
                        await RetryWithExponentialBackoff(
                            async () =>
                            {
                                using var connectCts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(30));
                                await vmConnectionSession.ServerItem.ServerWebSocket.ConnectAsync(uriServer, connectCts.Token);
                            },
                            "Server WebSocket Connect",
                            vmConnectionSession.Identifier);
                        if (vmConnectionSession.ServerItem.ServerWebSocket.State != WebSocketState.Open)
                            await ServerCannotStartConnect(vmConnectionSession);
                        await UpdateServerConnectionDb(vmConnectionSession, true);
                    }
                }
                catch (Exception ex)
                {
                    await ServerCannotStartConnectWithException(vmConnectionSession, ex);
                    await UpdateServerConnectionDb(vmConnectionSession, false);
                }
            }

            return (uriServer, hasValidServerUrl);
        }

        /// <summary>
        /// Faz 1b: CPO WebSocket baglantisinı kurar. SSL strict → fallback bypass.
        /// </summary>
        private async Task<(Uri uriCpo, bool hasValidCpoUrl)> SetupCpoConnection(
            VmConnectionSessionDto vmConnectionSession,
            string ocppUrlAddress)
        {
            bool hasValidCpoUrl = !string.IsNullOrWhiteSpace(ocppUrlAddress);
            if (!hasValidCpoUrl)
            {
                _logger.LogInformation($"CPO URL not configured for device {vmConnectionSession.Identifier}. Skipping CPO connection.");
                return (null, false);
            }

            Uri uriCpo = new Uri(ocppUrlAddress + vmConnectionSession.Identifier);
            _logger.LogInformation($"Attempting CPO connection for device {vmConnectionSession.Identifier} to URL: {uriCpo}");

            if (vmConnectionSession.CpoItem == null ||
                vmConnectionSession.CpoItem.CpoWebSocket == null ||
                (vmConnectionSession.CpoItem.CpoWebSocket.State != WebSocketState.Open &&
                 vmConnectionSession.CpoItem.CpoWebSocket.State != WebSocketState.Connecting))
            {
                try
                {
                    GetOrAddVmCpoConnectionResponseDto? vmCpoConnection = null;
                    GetOrAddVmCpoConnectionRequestDto getOrAddVmCpoConnectionRequest = new GetOrAddVmCpoConnectionRequestDto();
                    getOrAddVmCpoConnectionRequest.VmDeviceConnectionId = vmConnectionSession.VmDeviceConnectionId;
                    getOrAddVmCpoConnectionRequest.Date = DateTime.Now;
                    using (var scope = _services.CreateScope())
                    {
                        IVmCpoConnectionManagementService vmCpoConnectionManagementService = scope.ServiceProvider.GetRequiredService<IVmCpoConnectionManagementService>();
                        vmCpoConnection = await vmCpoConnectionManagementService.GetOrAddVmCpoConnection(getOrAddVmCpoConnectionRequest);
                    }

                    if (vmCpoConnection != null)
                    {
                        try
                        {
                            vmConnectionSession.CpoCancellationTokenSource?.Cancel();
                            vmConnectionSession.CpoCancellationTokenSource?.Dispose();
                        }
                        catch (Exception disposeEx) { _logger.LogDebug(disposeEx, $"[CPO SETUP] Non-critical dispose failed for {vmConnectionSession.Identifier}"); }
                        vmConnectionSession.CpoCancellationTokenSource = new CancellationTokenSource();
                        vmConnectionSession.CpoItem = new VmConnectionCpoItemDto();
                        bool connectedWithStrictSsl = false;

                        // Once strict SSL dener, basarisiz olursa bypass ile tekrar dener
                        try
                        {
                            _logger.LogInformation($"Attempting CPO connection with STRICT SSL validation - Device: {vmConnectionSession.Identifier}");
                            int attempt = 0; int delay = 1000; const int maxAttempts = 5;
                            while (attempt < maxAttempts)
                            {
                                try
                                {
                                    vmConnectionSession.CpoItem.CpoWebSocket?.Dispose();
                                    vmConnectionSession.CpoItem.CpoWebSocket = new ClientWebSocket();
                                    vmConnectionSession.CpoItem.CpoWebSocket.Options.AddSubProtocol("ocpp1.6");
                                    vmConnectionSession.CpoItem.CpoWebSocket.Options.KeepAliveInterval = TimeSpan.Zero;
                                    using var connectCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                                    await vmConnectionSession.CpoItem.CpoWebSocket.ConnectAsync(uriCpo, connectCts.Token);
                                    if (vmConnectionSession.CpoItem.CpoWebSocket.State == WebSocketState.Open)
                                    {
                                        connectedWithStrictSsl = true;
                                        _logger.LogInformation($"CPO connected with VALID SSL certificate - Device: {vmConnectionSession.Identifier}");
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    attempt++;
                                    if (attempt >= maxAttempts) throw;
                                    _logger.LogWarning(ex, $"CPO Strict SSL attempt {attempt} failed for {vmConnectionSession.Identifier}. Retrying in {delay}ms");
                                    await Task.Delay(delay);
                                    delay = Math.Min(delay * 2, 8000);
                                }
                            }
                        }
                        catch (Exception sslEx) when (
                            sslEx.InnerException is System.Security.Authentication.AuthenticationException ||
                            sslEx.Message.Contains("SSL") || sslEx.Message.Contains("certificate"))
                        {
                            // SSL basarisiz — bypass ile yeniden dene
                            _logger.LogWarning($"CPO SSL validation FAILED - Device: {vmConnectionSession.Identifier}. Retrying with BYPASSED SSL.");
                            try
                            {
                                vmConnectionSession.CpoCancellationTokenSource = new CancellationTokenSource();
                                int attempt = 0; int delay = 1000; const int maxAttempts = 5;
                                while (attempt < maxAttempts)
                                {
                                    try
                                    {
                                        vmConnectionSession.CpoItem.CpoWebSocket?.Dispose();
                                        vmConnectionSession.CpoItem.CpoWebSocket = new ClientWebSocket();
                                        vmConnectionSession.CpoItem.CpoWebSocket.Options.RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true;
                                        vmConnectionSession.CpoItem.CpoWebSocket.Options.AddSubProtocol("ocpp1.6");
                                        vmConnectionSession.CpoItem.CpoWebSocket.Options.KeepAliveInterval = TimeSpan.Zero;
                                        using var bypassCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                                        await vmConnectionSession.CpoItem.CpoWebSocket.ConnectAsync(uriCpo, bypassCts.Token);
                                        if (vmConnectionSession.CpoItem.CpoWebSocket.State == WebSocketState.Open)
                                        {
                                            _logger.LogWarning($"CPO connected with BYPASSED SSL - Device: {vmConnectionSession.Identifier}");
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        attempt++;
                                        if (attempt >= maxAttempts) throw;
                                        await Task.Delay(delay);
                                        delay = Math.Min(delay * 2, 8000);
                                    }
                                }
                            }
                            catch (Exception bypassEx)
                            {
                                _logger.LogError($"CPO connection FAILED even with SSL bypass - Device: {vmConnectionSession.Identifier}");
                                throw;
                            }
                        }

                        if (vmConnectionSession.CpoItem.CpoWebSocket.State != WebSocketState.Open)
                            await CpoCannotStartConnect(vmConnectionSession);
                        await UpdateCpoConnectionDb(vmConnectionSession, true);
                    }
                }
                catch (Exception ex)
                {
                    await CpoCannotStartConnectWithException(vmConnectionSession, ex);
                    await UpdateCpoConnectionDb(vmConnectionSession, false);
                }
            }

            return (uriCpo, hasValidCpoUrl);
        }
    }
}
