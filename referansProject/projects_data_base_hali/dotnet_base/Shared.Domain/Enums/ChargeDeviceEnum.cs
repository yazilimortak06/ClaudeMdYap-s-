using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Enums.ApiEnums
{
    public enum ChargeDeviceStateEnum
    {
        ACTIVE = 1,
        IN_CARE = 2,
        DEFECTIVE = 3,
        UNKNOW = 4
    }
    public enum ChargeDeviceInstantStateEnum
    {
        AVAILABLE = 1, // uygun
        UNAVAILABLE = 2, // uygun değil
        FAULTED = 3, // arızalı
    }
    public enum ChargeDeviceConnectorStateEnum
    {
        ACTIVE = 1,
        IN_CARE = 2,
        DEFECTIVE = 3,
        UNKNOW = 4
    }
    public enum ChargeDeviceConnectorInstantStateEnum
    {
        [Description("AVAILABLE")]
        AVAILABLE = 1, // uygun
        [Description("UNAVAILABLE")]
        UNAVAILABLE = 2, // uygun değil
        [Description("PREPARING")]
        PREPARING = 3, // hazırlanıyor
        [Description("IN_PROCESS")]
        IN_PROCESS = 4, // şarj işleminde
        [Description("FAULTED")]
        FAULTED = 5, // arızalı
        [Description("OCCUPIED")]
        OCCUPIED = 6, // Dolu
    }
    public enum ChargeDevicePowerTypeEnum
    {
        AC = 1,
        DC = 2
    }
    public enum ChargeDeviceSocketTypeEnum
    {
        AC_TYPE_2 = 1,
        AC_TYPE_2_PLUG = 2,
        DC_CCS = 3,
        DC_CHADEMO = 4
    }
    public enum ChargeDeviceMarkEnum
    {
        TELTONIKA = 1,
        SINEXCEL = 2,
        WAT = 3,
        POWERCHARGE = 4,
        ASPOWER = 5,
        STARTCHARGE = 6,
        CHARGE_TECHNOLOGY = 7,
        PIWIN = 8,
        CIRCONTROL = 9,
        UMAYTECH = 10,
        SIEMENS = 11,
        SCHNEIDER = 12,
        OTHER = 13,
    }
    public enum ChargeDeviceReservationStateEnum
    {
        RECEIVED = 1,
        COMPLETED = 2,
        CANCELLED = 3,
        TIME_EXPIRED = 4,
    }
    public enum ChargeDeviceReservationResultStateEnum
    {
        CANCELLED = 1,
        CHARGE_START = 2,
        COMPLETED = 3
    }
    public enum ChargeDeviceConnectorWaitingPriceStateEnum
    {
        NONE = 1,
        WAITING = 2,
    }
    public enum ChargeDeviceReservationTypeEnum
    {
        NORMAL = 1,
        ROUTE = 2,
    }
}
