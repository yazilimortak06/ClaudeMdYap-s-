// ============================================================
// sarj_vm_backend_dotnet — TUM ENTITY SINIFLARI
// Kaynak: src/Shared/Shared.Domain/Entities/VmEntities/
// Schema: RotaWatt (PostgreSQL)
// ============================================================

using FrameworkCore.Bases.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Domain.Enums.VmEnums;
using Shared.Domain.Enums.VmPanelEnums;

// ---- VmCpo.cs ----

namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmCpo", Schema = "RotaWatt")]
    public class VmCpo : BaseEntity
    {
        public string GuiId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OcppUrlAddress { get; set; }
        public string CpoSubProtocol { get; set; }
        public VmIntegrationConfig VmIntegrationConfig { get; set; }
        public virtual ICollection<VmStation> VmStation { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<VmPanelAdmin> VmPanelAdmin { get; set; }
    }
}

// ---- VmStation.cs ----

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
        public DateTime? UpdatedDate { get; set; }
    }
}

// ---- VmDevice.cs ----

namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmDevice", Schema = "RotaWatt")]
    public class VmDevice : BaseEntity
    {
        public string GuiId { get; set; }
        public string DeviceServerGuiId { get; set; }
        public string Identifier { get; set; }
        [ForeignKey("VmDevice_VmStation")]
        public long VmStationId { get; set; }
        public VmStation VmStation { get; set; }
        public VmDeviceConnection? VmDeviceConnection { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ChargeDeviceMarkEnum? ChargeDeviceMarkId { get; set; }
        public virtual ICollection<VmConnector> VmConnector { get; set; }
    }
}

// ---- VmConnector.cs ----

namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmConnector", Schema = "RotaWatt")]
    public class VmConnector : BaseEntity
    {
        [MaxLength(36)]
        public string GuiId { get; set; }
        public string Identifier { get; set; }
        public int ConnectorNo { get; set; }
        [ForeignKey("VmConnector_VmDevice")]
        public long VmDeviceId { get; set; }
        public VmDevice VmDevice { get; set; }
        [ForeignKey("VmConnector_VmConnectorPowerType")]
        public long VmConnectorPowerTypeId { get; set; }
        public VmConnectorPowerType VmConnectorPowerType { get; set; }
        public VmConnectorConnection? VmConnectorConnection { get; set; }
        public virtual ICollection<VmStartTransaction> VmStartTransaction { get; set; }
    }
}

// ---- VmConnectorPowerType.cs ----

namespace Shared.Domain.Entities.VmEntities.VmCpoModule
{
    [Table("VmConnectorPowerType", Schema = "RotaWatt")]
    public class VmConnectorPowerType : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public virtual ICollection<VmConnector> VmConnector { get; set; }
    }
}

// ---- VmDeviceConnection.cs ----

namespace Shared.Domain.Entities.VmEntities.VmDeviceConnectionModule
{
    [Table("VmDeviceConnection", Schema = "RotaWatt")]
    public class VmDeviceConnection : BaseEntity
    {
        public string GuiId { get; set; }
        public bool ConnectionState { get; set; }
        [ForeignKey("VmDeviceConnection_VmDevice")]
        public long VmDeviceId { get; set; }
        public VmDevice VmDevice { get; set; }
        public ChargeDeviceInstantStateEnum? InstantState { get; set; }
        public VmCpoConnection? VmCpoConnection { get; set; }
        public VmServerConnection? VmServerConnection { get; set; }
        public string? ConnectedIpAddress { get; set; }
        public DateTime? ConnectionDate { get; set; }
        public DateTime? DisconnectionDate { get; set; }
        public string? DisconnectionMessage { get; set; }
        public DateTime? HearthBeatDate { get; set; }
        public DateTime? LastInstantStateUpdatedDate { get; set; }
        public DateTime? LastMessageReceiveDate { get; set; }
    }
}

// ---- VmCpoConnection.cs ----

namespace Shared.Domain.Entities.VmEntities.VmConnectionModule
{
    [Table("VmCpoConnection", Schema = "RotaWatt")]
    public class VmCpoConnection : BaseEntity
    {
        public string GuiId { get; set; }
        public bool ConnectionState { get; set; }
        public DateTime ConnectionDate { get; set; }
        public DateTime DisconnectionDate { get; set; }
        public string DisconnectionMessage { get; set; }
        public string ConnectedCpoOcppUrlAddress { get; set; }
        [ForeignKey("VmCpoConnection_VmDeviceConnection")]
        public long VmDeviceConnectionId { get; set; }
        public VmDeviceConnection VmDeviceConnection { get; set; }
    }
}

// ---- VmServerConnection.cs ----

namespace Shared.Domain.Entities.VmEntities.VmConnectionModule
{
    [Table("VmServerConnection", Schema = "RotaWatt")]
    public class VmServerConnection : BaseEntity
    {
        public string GuiId { get; set; }
        public bool ConnectionState { get; set; }
        public DateTime ConnectionDate { get; set; }
        public DateTime DisconnectionDate { get; set; }
        public string DisconnectionMessage { get; set; }
        public string ConnectedServerUrlAddress { get; set; }
        [ForeignKey("VmServerConnection_VmDeviceConnection")]
        public long VmDeviceConnectionId { get; set; }
        public VmDeviceConnection VmDeviceConnection { get; set; }
    }
}

// ---- VmConnectorConnection.cs ----

namespace Shared.Domain.Entities.VmEntities.VmConnectionModule
{
    [Table("VmConnectorConnection", Schema = "RotaWatt")]
    public class VmConnectorConnection : BaseEntity
    {
        public string GuiId { get; set; }
        [ForeignKey("VmConnectorConnection_VmConnector")]
        public long VmConnectorId { get; set; }
        public VmConnector VmConnector { get; set; }
        public ChargeDeviceConnectorInstantStateEnum? InstantState { get; set; }
        public DateTime? LastInstantStateUpdatedDate { get; set; }
    }
}

// ---- VmStartTransaction.cs ----

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
        public TransactionStateEnum State { get; set; }
        [MaxLength(64)]
        public string TransactionIdTag { get; set; }
        [ForeignKey("VmStartTransaction_VmConnector")]
        public long VmConnectorId { get; set; }
        public VmConnector VmConnector { get; set; }
        [ForeignKey("VmStartTransaction_VmIdTagInfo")]
        public long? VmIdTagInfoId { get; set; }
        public VmIdTagInfo? VmIdTagInfo { get; set; }
        public long? CpoTransactionId { get; set; }
        public long? ServerTransactionId { get; set; }
        public string? CpoIdTag { get; set; }
        public string? ServerIdTag { get; set; }
        public double? LoadedKw { get; set; }
        public virtual ICollection<VmMeterValue> VmMeterValue { get; set; }
    }
}

// ---- VmMeterValue.cs ----

namespace Shared.Domain.Entities.VmEntities.VmTransactionModule
{
    [Table("VmMeterValue", Schema = "RotaWatt")]
    public class VmMeterValue : BaseEntity
    {
        public string GuiId { get; set; }
        public string MessageUniqueId { get; set; }
        public double ChargeAmountOfEnergy { get; set; }  // sarj edilen enerji miktari (kWh)
        public double TotalChargePower { get; set; }       // toplam sarj gucu (kW)
        public DateTime Timestamp { get; set; }
        public int ConnectorNo { get; set; }
        public long TransactionId { get; set; }
        [ForeignKey("VmMeterValue_VmStartTransaction")]
        public long? VmStartTransactionId { get; set; }
        public VmStartTransaction? VmStartTransaction { get; set; }
        public double? StateOfCharge { get; set; }         // Sarj durumu (%)
        public SampledValueContextEnum? SampledValueContext { get; set; }
        public virtual ICollection<VmMeterSampledValue> VmMeterSampledValue { get; set; }
    }
}

// ---- VmMeterSampledValue.cs ----

namespace Shared.Domain.Entities.VmEntities.VmTransactionModule
{
    [Table("VmMeterSampledValue", Schema = "RotaWatt")]
    public class VmMeterSampledValue : BaseEntity
    {
        public string GuiId { get; set; }
        public string Value { get; set; }
        public string Measurand { get; set; }
        public string Unit { get; set; }
        public string Phase { get; set; }
        public string Location { get; set; }
        public string Format { get; set; }
        public string Context { get; set; }
        [ForeignKey("VmMeterSampledValue_VmMeterValue")]
        public long VmMeterValueId { get; set; }
        public VmMeterValue VmMeterValue { get; set; }
    }
}

// ---- VmCommandMessage.cs ----

namespace Shared.Domain.Entities.VmEntities.VmCommandMessageModule
{
    [Table("VmCommandMessage", Schema = "RotaWatt")]
    public class VmCommandMessage : BaseEntity
    {
        public string GuiId { get; set; }
        public string Data { get; set; }
        public string Identifier { get; set; }
        public string MessageUniqueId { get; set; }
        public string SenderIpAddress { get; set; }
        public VmCommandMessageTypeEnum SenderType { get; set; }
        public VmCommandMessageTypeEnum ReceivedType { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("VmCommandMessage_VmDeviceConnection")]
        public long? VmDeviceConnectionId { get; set; }
        public VmDeviceConnection? VmDeviceConnection { get; set; }
        public string? MessageDescription { get; set; }
        public VmCommandMessageTopicEnum? Topic { get; set; }
    }
}

// ---- VmPanelAdmin.cs ----

namespace Shared.Domain.Entities.VmEntities.VmPanelAdminModule
{
    [Table("VmPanelAdmin", Schema = "RotaWatt")]
    public class VmPanelAdmin : BaseEntity
    {
        [StringLength(36)] public string AdminUserGuid { get; set; }
        [StringLength(24)] public string UserName { get; set; }
        [StringLength(36)] public string Name { get; set; }
        [StringLength(36)] public string Surname { get; set; }
        [StringLength(36)] public string Mail { get; set; }
        public bool IsActive { get; set; }
        [StringLength(14)] public string Phone { get; set; }
        public AdminManagmentTypeEnum AdminManagmentType { get; set; }
        public string Md5Password { get; set; }
        [ForeignKey("VmPanelAdmin_VmCpo")]
        public long? VmCpoId { get; set; }
        public VmCpo? VmCpo { get; set; }
    }
}

// ---- VmPanelRootAdmin.cs ----

namespace Shared.Domain.Entities.VmEntities.VmPanelAdminModule
{
    [Table("VmPanelRootAdmin", Schema = "RotaWatt")]
    public class VmPanelRootAdmin : BaseEntity
    {
        [StringLength(36)] public string RootAdminUserGuid { get; set; }
        [StringLength(24)] public string UserName { get; set; }
        [StringLength(36)] public string Name { get; set; }
        [StringLength(36)] public string Surname { get; set; }
        [StringLength(36)] public string Mail { get; set; }
        [StringLength(14)] public string Phone { get; set; }
        public string Md5Password { get; set; }
    }
}

// ---- PanelLoginSession.cs ----

namespace Shared.Domain.Entities.VmEntities.VmPanelAdminModule
{
    [Table("PanelLoginSession", Schema = "RotaWatt")]
    public class PanelLoginSession : BaseEntity
    {
        public string SessionKey { get; set; }
        public string SessionJwtKey { get; set; }
        public string RememberKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastVerificationDate { get; set; }
        public long PanelDeviceId { get; set; }
        public PanelDevice PanelDevice { get; set; }
        public long? VmPanelAdminId { get; set; }
        public VmPanelAdmin? VmPanelAdmin { get; set; }
        public long? VmPanelRootAdminId { get; set; }
        public VmPanelRootAdmin? VmPanelRootAdmin { get; set; }
    }
}

// ---- PanelLoginAttention.cs ----

namespace Shared.Domain.Entities.VmEntities.VmPanelAdminModule
{
    [Table("PanelLoginAttention", Schema = "RotaWatt")]
    public class PanelLoginAttention : BaseEntity
    {
        public string UserNameMd5 { get; set; }
        public int FailedCount { get; set; }
        public DateTime AttentionDate { get; set; }
        public string LoginFormKey { get; set; }
        public long PanelDeviceId { get; set; }
        public PanelDevice PanelDevice { get; set; }
    }
}

// ---- PanelDevice.cs ----

namespace Shared.Domain.Entities.VmEntities.VmPanelAdminModule
{
    [Table("PanelDevice", Schema = "RotaWatt")]
    public class PanelDevice : BaseEntity
    {
        public string Guid { get; set; }
        public string UserAgentData { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

// ---- VmIdTagInfo.cs ----

namespace Shared.Domain.Entities.VmEntities.VmIdTagModule
{
    [Table("VmIdTagInfo", Schema = "RotaWatt")]
    public class VmIdTagInfo : BaseEntity
    {
        public string GuiId { get; set; }
        public string IdTag { get; set; }
        public string Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ParentIdTag { get; set; }
    }
}

// ---- VmEvccId.cs ----

namespace Shared.Domain.Entities.VmEntities.VmEvccIdModule
{
    [Table("VmEvccId", Schema = "RotaWatt")]
    public class VmEvccId : BaseEntity
    {
        public string GuiId { get; set; }
        public string EvccId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

// ---- VmParameter.cs ----

namespace Shared.Domain.Entities.VmEntities.VmParameterModule
{
    [Table("VmParameter", Schema = "RotaWatt")]
    public class VmParameter : BaseEntity
    {
        public bool IsVmActive { get; set; }
        public string ServerUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

// ---- VmIntegrationConfig.cs ----

namespace Shared.Domain.Entities.VmEntities.VmIntegrationConfigModuleModule
{
    [Table("VmIntegrationConfig", Schema = "RotaWatt")]
    public class VmIntegrationConfig : BaseEntity
    {
        public string GuiId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [ForeignKey("VmIntegrationConfig_VmCpo")]
        public long VmCpoId { get; set; }
        public VmCpo VmCpo { get; set; }
    }
}
