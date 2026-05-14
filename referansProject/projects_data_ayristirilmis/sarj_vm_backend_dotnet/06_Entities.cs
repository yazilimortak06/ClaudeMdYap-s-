// KAYNAK: E:\Projeler\Backend\rotawattvmbackend-develop (1)\rotawattvmbackend-develop\src\Shared\Shared.Domain\Entities\

// ============================================================
// VmCpo.cs — CPO (Charge Point Operator) entity
// KAYNAK: ..\Entities\VmEntities\VmCpoModule\VmCpo.cs
// ============================================================
using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.VmEntities.VmIntegrationConfigModule;
using Shared.Domain.Entities.VmEntities.VmPanelAdminModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmCpo", Schema = "RotaWatt")]
    public class VmCpo : BaseEntity
    {
        public string GuiId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OcppUrlAddress { get; set; }         // CPO'nun OCPP WebSocket URL'i
        public string CpoSubProtocol { get; set; }         // ocpp1.6 / ocpp1.6j
        public VmIntegrationConfig VmIntegrationConfig { get; set; }
        public virtual ICollection<VmStation> VmStation { get; set; }
#nullable enable
        public DateTime? UpdatedDate { get; set; }
#nullable disable
        public virtual ICollection<VmPanelAdmin> VmPanelAdmin { get; set; }
    }
}

// ============================================================
// VmStation.cs — İstasyon (şarj noktası grubu) entity
// KAYNAK: ..\Entities\VmEntities\VmCpoModule\VmStation.cs
// ============================================================
namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmStation", Schema = "RotaWatt")]
    public class VmStation : BaseEntity
    {
        public string GuiId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("VmStation_VmCpo")]
        public long VmCpoId { get; set; }
        public VmCpo VmCpo { get; set; }
        public virtual ICollection<VmDevice> VmDevice { get; set; }
#nullable enable
        public DateTime? UpdatedDate { get; set; }
#nullable disable
    }
}

// ============================================================
// VmDevice.cs — Şarj cihazı entity
// KAYNAK: ..\Entities\VmEntities\VmCpoModule\VmDevice.cs
// ============================================================
namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmDevice", Schema = "RotaWatt")]
    public class VmDevice : BaseEntity
    {
        public string GuiId { get; set; }
        public string DeviceServerGuiId { get; set; }      // Server'daki cihaz GUID'i
        public string Identifier { get; set; }              // OCPP identifier (WebSocket URL parametresi)
        [ForeignKey("VmDevice_VmStation")]
        public long VmStationId { get; set; }
        public VmStation VmStation { get; set; }
#nullable enable
        public VmDeviceConnection? VmDeviceConnection { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ChargeDeviceMarkEnum? ChargeDeviceMarkId { get; set; }   // Cihaz markası
#nullable disable
        public virtual ICollection<VmConnector> VmConnector { get; set; }
    }
}

// ============================================================
// VmStartTransaction.cs — Şarj işlemi entity
// KAYNAK: ..\Entities\VmEntities\VmTransactionModule\VmStartTransaction.cs
// ============================================================
using Shared.Domain.Entities.VmEntities.VmIdTagModule;
using Shared.Domain.Enums.VmEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Entities.VmEntities.VmTransactionModule
{
    [Table("VmStartTransaction", Schema = "RotaWatt")]
    public class VmStartTransaction : BaseEntity
    {
        public string GuiId { get; set; }
        public string MessageUniqueId { get; set; }
        public double MeterStart { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public TransactionStateEnum State { get; set; }     // PENDING, ACTIVE, COMPLETED, FAILED
        [MaxLength(64)]
        public string TransactionIdTag { get; set; }
        [ForeignKey("VmStartTransaction_VmConnector")]
        public long VmConnectorId { get; set; }
        public VmConnector VmConnector { get; set; }
#nullable enable
        [ForeignKey("VmStartTransaction_VmIdTagInfo")]
        public long? VmIdTagInfoId { get; set; }
        public VmIdTagInfo? VmIdTagInfo { get; set; }
        public long? CpoTransactionId { get; set; }         // CPO'dan gelen transaction ID
        public long? ServerTransactionId { get; set; }      // Server'dan gelen transaction ID
        public string? CpoIdTag { get; set; }
        public string? ServerIdTag { get; set; }
        public double? LoadedKw { get; set; }               // Yüklenen kW miktarı
#nullable disable
        public virtual ICollection<VmMeterValue> VmMeterValue { get; set; }
    }
}

// ============================================================
// VmCommandMessage.cs — OCPP komut mesajı log entity
// KAYNAK: ..\Entities\VmEntities\VmCommandMessageModule\VmCommandMessage.cs
// ============================================================
namespace Shared.Domain.Entities.VmEntities.VmCommandMessageModule
{
    [Table("VmCommandMessage", Schema = "RotaWatt")]
    public class VmCommandMessage : BaseEntity
    {
        public string GuiId { get; set; }
        public long? VmDeviceConnectionId { get; set; }
        public string Data { get; set; }
        public VmCommandMessageTypeEnum SenderType { get; set; }    // DEVICE, VM, CPO, SERVER
        public VmCommandMessageTypeEnum ReceivedType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string MessageDescription { get; set; }
        public VmCommandMessageTopicEnum Topic { get; set; }        // CONNECTED, HEARTBEAT, START_TX, vb.
        public string SenderIpAddress { get; set; }
    }
}

// ============================================================
// VmDeviceConnection.cs — Cihaz bağlantı durumu entity
// KAYNAK: ..\Entities\VmEntities\VmConnectionModule\VmDeviceConnection.cs
// ============================================================
namespace Shared.Domain.Entities.VmEntities.VmDeviceConnectionModule
{
    [Table("VmDeviceConnection", Schema = "RotaWatt")]
    public class VmDeviceConnection : BaseEntity
    {
        public DateTime? ConnectionDate { get; set; }
        public DateTime? DisconnectionDate { get; set; }
        public bool ConnectionState { get; set; }
        public string SelectedSubProtocol { get; set; }
        [ForeignKey("VmDeviceConnection_VmDevice")]
        public long VmDeviceId { get; set; }
        public VmDevice VmDevice { get; set; }
    }
}

// ============================================================
// ExceptionLog.cs — Hata log entity
// KAYNAK: ..\Entities\VmLogEntities\ExceptionLog.cs
// ============================================================
namespace Shared.Domain.Entities.VmLogEntities
{
    public class ExceptionLog : BaseEntity
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Path { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

// ============================================================
// RequestResponseLog.cs — HTTP request/response log entity
// KAYNAK: ..\Entities\VmLogEntities\RequestResponseLog.cs
// ============================================================
namespace Shared.Domain.Entities.VmLogEntities
{
    public class RequestResponseLog : BaseEntity
    {
        public string RequestPath { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
