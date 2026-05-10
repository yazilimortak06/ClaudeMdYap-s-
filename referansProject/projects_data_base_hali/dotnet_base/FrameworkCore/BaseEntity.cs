// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\BaseEntities\BaseEntity.cs
// Ek: IEntity, IHasId, IHasDeleted interface'leri (DataProperties klasöründen)
// Kaynak (interface'ler): E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\FrameworkCore\DataProperties\

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.FrameworkCore.DataProperties
{
    /// <summary>
    /// Id zorunluluğunu tanımlar.
    /// </summary>
    public interface IHasId
    {
        long Id { get; set; }
    }

    /// <summary>
    /// Soft-delete desteği için Deleted property'si zorunluluğunu tanımlar.
    /// </summary>
    public interface IHasDeleted
    {
        bool Deleted { get; set; }
    }

    /// <summary>
    /// Tüm entity'lerin uyması gereken temel interface.
    /// </summary>
    public interface IEntity : IHasId, IHasDeleted
    {
    }

    /// <summary>
    /// DTO'lar için marker interface.
    /// </summary>
    public interface IDto
    {
    }
}

namespace FrameworkCore.Bases.BaseEntities
{
    using FrameworkCore.FrameworkCore.DataProperties;

    /// <summary>
    /// Bir Entity'nin alması gereken en temel class.
    /// Id ve soft-delete (Deleted) property'lerini içerir.
    /// </summary>
    public class BaseEntity : IEntity
    {
        public long Id { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
