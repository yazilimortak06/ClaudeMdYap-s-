// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\BaseEntities\BaseEntity.cs
// Ve: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\FrameworkCore\DataProperties\
//
// NOT: Shared.Domain projesi kendi entity'leri için ayrı bir base entity tanımlamaz;
//      doğrudan FrameworkCore.Bases.BaseEntities.BaseEntity kullanır.
//      Burada referans için interface hiyerarşisi özetlenmiştir.

namespace FrameworkCore.FrameworkCore.DataProperties
{
    /// <summary>
    /// Entity kimlik zorunluluğu - long Id.
    /// </summary>
    public interface IHasId
    {
        long Id { get; set; }
    }

    /// <summary>
    /// Soft-delete desteği - Deleted flag.
    /// </summary>
    public interface IHasDeleted
    {
        bool Deleted { get; set; }
    }

    /// <summary>
    /// Tüm entity'lerin uygulaması gereken temel interface.
    /// IHasId + IHasDeleted bileşimi.
    /// </summary>
    public interface IEntity : IHasId, IHasDeleted
    {
    }
}

namespace FrameworkCore.Bases.BaseEntities
{
    using FrameworkCore.FrameworkCore.DataProperties;

    /// <summary>
    /// Tüm domain entity'lerinin türeyeceği en temel sınıf.
    /// Id (long, PK) ve Deleted (soft-delete flag) içerir.
    ///
    /// Kullanım: public class MyEntity : BaseEntity { ... }
    /// </summary>
    public class BaseEntity : IEntity
    {
        public long Id { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
