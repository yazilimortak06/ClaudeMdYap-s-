using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.WrapperCore.Models
{
    /// <summary>
    /// Result model to contain data, result type, and errors
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
}
