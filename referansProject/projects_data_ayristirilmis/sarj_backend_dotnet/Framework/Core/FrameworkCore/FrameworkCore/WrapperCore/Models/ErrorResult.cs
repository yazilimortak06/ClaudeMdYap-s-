using FrameworkCore.FrameworkCore.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.WrapperCore.Models
{
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
