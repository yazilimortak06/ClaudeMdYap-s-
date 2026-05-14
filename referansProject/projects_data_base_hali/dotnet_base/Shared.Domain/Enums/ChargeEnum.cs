using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Enums.ApiEnums
{
    public enum ChargeStateEnum
    {
        [Description("Şarj Başlatılıyor")]
        PROCESS_STARTING = 1, // işlem başlatılıyor
        [Description("Şarj Oluyor")]
        PROCESS_START = 2, // işlem başladı
        [Description("Şarj Durduruluyor")]
        PROCESS_ENDING = 3, // işlem durduruluyor
        [Description("Ödeme Alınıyor")]
        PAYMENT_BEING_RECEIVED = 4, // işlem bitti,ödeme alınıyor
        [Description("Ödeme Başarısız")]
        PAYMENT_FAIL = 5, // ödeme başarısız
        [Description("İşlem Başarılı")]
        COMPLETED = 6, // işlem başarılı
        [Description("İşlem Başarısız")]
        FAILED = 7, // işlem başarısız
        [Description("İşlem Hesaplanıyor")]
        CALCULATING = 8, // işlem hesaplanıyor
    }
    public enum ChargeDurationTypeEnum
    {
        SECOND = 1,
        MINUTE = 2,
        HOUR = 3,
    }
    public enum ChargeFirmTypeEnum
    {
        [Description("RotaWatt")]
        ROTAWATT = 1,
        [Description("Firma")]
        FIRM = 2,
    }
    public enum DeviceSelectTypeEnum
    {
        QR = 1,
        SERIAL_NUMBER = 2,
        NFC = 3
    }
}
