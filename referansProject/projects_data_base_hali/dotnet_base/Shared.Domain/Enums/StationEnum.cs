using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Enums.ApiEnums
{
    public enum StationFilterStateEnum
    {
        AVAILABLE = 1, // şarj da değilse ve durumu aktifse 
        RESERVATIONED = 2, // kullanıcının rezervasyonu varsa
        NOT_AVAILABLE = 3 // şarj da veya aktif değilse
    }
    public enum StationFirmTypeEnum
    {
        ROTAWATT = 1,
        FIRM = 2
    }
    public enum StationStateEnum
    {
        AVAILABLE = 1,
        NOT_AVAILABLE = 2,
        BUSY = 3,
        UNKNOW = 4
    }
    public enum StationMobilOrderTypeEnum
    {
        STATION_NAME = 1,
        DISTANCE = 2,
        KW = 3,
        PRICE = 4,
        POINT = 5,
    }
}
