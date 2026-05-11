using FrameworkCore.Bases.BaseEntities;
using Shared.Domain.Entities.ApiEntities.ChargeDeviceModule;
using Shared.Domain.Entities.ApiEntities.FeedbackModule;
using Shared.Domain.Entities.ApiEntities.FirmModule;
using Shared.Domain.Entities.ApiEntities.GuestUserModule;
using Shared.Domain.Entities.ApiEntities.PaymentInfoModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Entities.ApiEntities.UserModule;
using Shared.Domain.Enums.ApiEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.ChargeModule
{
    [Table("Charge", Schema = "RotaWatt")]
    public class Charge : BaseEntity
    {
        public string GuiId { get; set; }
        public ChargeStateEnum State { get; set; }
        public double Kdv { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public decimal KwPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ConnectionId { get; set; }
        public ChargeFirmTypeEnum ChargeFirmType { get; set; }
        public string PluggedSocketMovementGuiId { get; set; }
        [ForeignKey("Charge_ChargeDeviceConnector")]
        public long ChargeDeviceConnectorId { get; set; }
        public ChargeDeviceConnector ChargeDeviceConnector { get; set; }
        //[ForeignKey("Charge_ChargingConnectorReading")]
        //public long ChargingConnectorReadingId { get; set; }
        //public ChargingConnectorReading ChargingConnectorReading { get; set; }
#nullable enable
        public double? MeterStart { get; set; }
        public double? ChargeVelocity { get; set; }
        [ForeignKey("Charge_ChargingConnectorReading")]
        public long? ChargingConnectorReadingId { get; set; }
        public ChargingConnectorReading? ChargingConnectorReading { get; set; }
        [ForeignKey("Charge_Firm")]
        public long? FirmId { get; set; }
        public Firm? Firm { get; set; }
        public string? FirmChargeGuiId { get; set; }
        public DateTime? AutomaticPaymentTrialDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public decimal? CalculatedPrice { get; set; }
        public double? LoadedKw { get; set; }
        public double? ChargePercentage { get; set; }
        public double? NeccessearyDuration { get; set; }
        public ChargeDurationTypeEnum NeccessearyDurationType { get; set; }
        public double? NeccessearyKW { get; set; }
        public decimal? NeccessearyPrice { get; set; }
        public string? TransactionGuiId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? EpdkProcessId { get; set; }
        public string? Description { get; set; }
        [ForeignKey("Charge_User")]
        public long? UserId { get; set; }
        public User? User { get; set; }
        public Feedback? Feedback { get; set; }
        public PaymentInfo? PaymentInfo { get; set; }
        public StationRating? StationRating { get; set; }

        //public UserAutomaticPaymentInfo? UserAutomaticPaymentInfo { get; set; }
        //[ForeignKey("ChargeProcess_ChargeStartSession")]
        //public long? ChargeStartSessionId { get; set; }
        //public ChargeStartSession? ChargeStartSession { get; set; }
        //[ForeignKey("ChargeProcess_ChargeDeviceReservation")]
        //public long? ChargeDeviceReservationId { get; set; }
        //public ChargeDeviceReservation? ChargeDeviceReservation { get; set; }

        //[ForeignKey("ChargeProcess_GuestUser")]
        //public long? GuestUserId { get; set; }
        //public GuestUser? GuestUser { get; set; }
        //[ForeignKey("ChargeProcess_GuestChargeProcessPreparing")]
        //public long? GuestChargeProcessPreparingId { get; set; }
        //public GuestChargeProcessPreparing? GuestChargeProcessPreparing { get; set; }
        //public StartTransactionProcess? StartTransactionProcess { get; set; }
#nullable disable
    }
}
