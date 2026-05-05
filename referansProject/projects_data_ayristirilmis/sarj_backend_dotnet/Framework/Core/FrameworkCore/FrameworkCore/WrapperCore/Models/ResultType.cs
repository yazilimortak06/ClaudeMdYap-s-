using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.WrapperCore.Models
{
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
}
