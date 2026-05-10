// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\BaseDtos\BaseDto.cs
// Ve: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\BaseDtos\ServerErrorDto.cs
// Ve: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\Dto\ApiDto\MobilPagingDtos\
//
// Bu dosya temel DTO sınıflarını ve paging DTO'larını içerir.
// Projede tüm DTO'lar BaseDto'dan türemez; çoğu direkt POCO'dur.
// BaseDto, IDto marker interface'ini implement eder.

using FrameworkCore.FrameworkCore.DataProperties;
using FrameworkCore.FrameworkCore.WrapperCore.Models;
using System.Collections.Generic;

namespace FrameworkCore.Bases.BaseDtos
{
    /// <summary>
    /// Tüm DTO'ların türeyebileceği temel sınıf. IDto marker interface'ini implement eder.
    /// </summary>
    public class BaseDto : IDto
    {
    }

    /// <summary>
    /// Sunucu hatalarını istemciye taşıyan DTO.
    /// ResultType her zaman Exception olarak set edilir.
    /// </summary>
    public class ServerErrorDto
    {
        public ResultType ResultType => ResultType.Exception;

        public ServerErrorDto()
        {
            MessageList = new List<string>();
        }

        public List<string> MessageList { get; set; }
        public int StateCode { get; set; }
    }
}

namespace Shared.Domain.Dto.ApiDto.MobilPagingDtos
{
    /// <summary>
    /// Mobil API'lar için offset-based pagination request DTO'su.
    /// Ofset: kaçıncı kayıttan başlanacak, Count: kaç kayıt getirilecek.
    /// </summary>
    public class MobilPagingRequestDto
    {
#nullable enable
        public int? Ofset { get; set; }
        public int? Count { get; set; }
#nullable disable
    }

    /// <summary>
    /// Sayfalı yanıtlarda toplam kayıt sayısını taşır.
    /// </summary>
    public class MobilPagingResponseDto
    {
        public int TotalRecord { get; set; }
    }
}
