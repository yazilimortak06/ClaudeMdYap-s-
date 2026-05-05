using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.WrapperCore.Models
{
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
}
