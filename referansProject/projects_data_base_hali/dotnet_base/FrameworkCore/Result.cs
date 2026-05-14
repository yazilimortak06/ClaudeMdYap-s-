// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\FrameworkCore\WrapperCore\Models\
// Dosyalar: Result.cs, ResultType.cs, SuccessResult.cs, ErrorResult.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.WrapperCore.Models
{
    /// <summary>
    /// API yanıtlarının taşıdığı sonuç tipi.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResultType
    {
        Ok,
        Error,
        ValidationError,
        Unauthorized,
        Exception,
        VersionInvalid
    }

    /// <summary>
    /// API yanıtlarını sarmaya yarayan temel result modeli.
    /// Data, ResultType ve hata bilgilerini içerir.
    /// </summary>
    public class Result<T>
    {
        public Result()
        {
        }

        public virtual ResultType ResultType { get; set; } = ResultType.Error;
        public virtual int ErrorCode { get; set; }

        [Required(AllowEmptyStrings = true)]
        public virtual string ErrorMessage { get; set; }

        [Required(AllowEmptyStrings = true)]
        public virtual T Data { get; set; }
    }

    /// <summary>
    /// Başarılı sonuçlar için kullanılan result sınıfı.
    /// ResultType her zaman Ok olarak set edilir.
    /// </summary>
    public class SuccessResult<T> : Result<T>
    {
        private readonly T _data;

        public SuccessResult(T data) : base()
        {
            _data = data;
            ErrorCode = 0;
            ErrorMessage = "";
        }

        public override ResultType ResultType => ResultType.Ok;
        public override T Data => _data;
    }

    /// <summary>
    /// Hatalı sonuçlar için kullanılan result sınıfı.
    /// Enum veya string hata mesajı ile oluşturulabilir.
    /// </summary>
    public class ErrorResult<T> : Result<T>
    {
#nullable enable
        private readonly T? _data;

        public ErrorResult(T? data) : base()
        {
            _data = data;
            ErrorCode = 0;
            ErrorMessage = "";
        }

        public ErrorResult(T? data, Enum error) : base()
        {
            _data = data;
            ErrorCode = (int)((object)error);
            ErrorMessage = error.ToDescriptionString();
        }

        public ErrorResult(T? data, string error) : base()
        {
            _data = data;
            ErrorCode = 0;
            ErrorMessage = error;
        }
#nullable disable

        public override ResultType ResultType => ResultType.Error;
        public override int ErrorCode { get; set; }
        public override string ErrorMessage { get; set; }
        public override T Data => _data;
    }
}
