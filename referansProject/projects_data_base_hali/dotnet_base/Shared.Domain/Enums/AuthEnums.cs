// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\GeneralEnums\AuthenticationType.cs
// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\GeneralEnums\AuthorizationByAdminType.cs
// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\src\Shared\Shared.Domain\Enums\LogEnums\EntityProcessTypeEnum.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.GeneralEnums
{
    /// <summary>
    /// Endpoint'in kimlik doğrulama gerekliliğini belirler.
    /// ALL: herkes erişebilir, ONLY_USER: sadece kimliği doğrulanmış kullanıcılar.
    /// </summary>
    public enum AuthenticationType
    {
        ALL = 1,
        ONLY_USER = 2,
    }

    /// <summary>
    /// Admin yetki kontrolü tipi.
    /// Token API'deki yetkilendirme attribute'larında kullanılır.
    /// </summary>
    public enum AuthorizationByAdminType
    {
        ONLY_ROOT = 1,
        ONLY_MAIN_ADMIN = 2,
        ROOT_OR_MAIN_ADMIN = 3,
        ALL = 4
    }
}

namespace Shared.Domain.Enums.LogEnums
{
    /// <summary>
    /// Entity üzerinde yapılan işlemin türü.
    /// Log kayıtlarında işlem tipini belirtmek için kullanılır.
    /// </summary>
    public enum EntityProcessTypeEnum
    {
        ADD = 1,
        UPDATE = 2,
        DELETE = 3
    }
}
