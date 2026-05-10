// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\FrameworkCore\WrapperCore\ResultExtensions.cs

using FrameworkCore.FrameworkCore.WrapperCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.WrapperCore
{
    /// <summary>
    /// Controller'larda Result<T> nesnelerinden ActionResult üretmek için extension metodlar.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Ok ise 200 Data döner, Error ise 400 Result döner.
        /// Panel/Web API'larında kullanılır.
        /// </summary>
        public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.ResultType == ResultType.Ok)
            {
                return controller.Ok(result.Data);
            }
            else
            {
                return controller.BadRequest(result);
            }
        }

        /// <summary>
        /// HttpClient çağrılarından dönen Result'ı olduğu gibi 200 ile döner.
        /// Caller taraf result'ı kendi yorumlar.
        /// </summary>
        public static ActionResult FromHttpClientResult<T>(this ControllerBase controller, Result<T> result)
        {
            return controller.Ok(result);
        }

        /// <summary>
        /// HttpClient çağrısından dönen ham data'yı 200 ile döner.
        /// </summary>
        public static ActionResult FromHttpClientDataResult<T>(this ControllerBase controller, T result)
        {
            return controller.Ok(result);
        }

        /// <summary>
        /// Mobil API'lar için: Ok ise 200 Result döner, Error ise 400 Result döner.
        /// Mobil client Result wrapper'ı bekler.
        /// </summary>
        public static ActionResult FromMobilResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.ResultType == ResultType.Ok)
            {
                return controller.Ok(result);
            }
            else
            {
                return controller.BadRequest(result);
            }
        }
    }
}
