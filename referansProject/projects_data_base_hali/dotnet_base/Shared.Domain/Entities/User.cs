using FrameworkCore.Bases.BaseEntities;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Entities.ApiEntities.CarsModule;
using Shared.Domain.Entities.ApiEntities.ChargeModule;
using Shared.Domain.Entities.ApiEntities.DebitCardModule;
using Shared.Domain.Entities.ApiEntities.MessageModule;
using Shared.Domain.Entities.ApiEntities.PaymentInfoModule;
using Shared.Domain.Entities.ApiEntities.RouteProcessModule;
using Shared.Domain.Entities.ApiEntities.StationModule;
using Shared.Domain.Entities.ApiEntities.SupportModule;
using Shared.Domain.Entities.ApiEntities.UserAdressModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Entities.ApiEntities.UserModule
{
    [Table("User", Schema = "RotaWatt")]
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneCountryCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsMailSubscription { get; set; }
        public string MobilUserGuiId { get; set; }
        public DateTime BirthDate { get; set; }
        public string TcNumber { get; set; }
#nullable enable
        public string? UserRemoveSmsGuiId { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool? IsActive { get; set; }
#nullable disable
        public virtual ICollection<StationRating> StationRating { get; set; }
        public virtual ICollection<DebitCard> DebitCard { get; set; }
        public virtual ICollection<UserAdress> UserAdress { get; set; }
        public virtual ICollection<ProcessingUserAdress> ProcessingUserAdress { get; set; }
        public virtual ICollection<Charge> Charge { get; set; }
        public virtual ICollection<PaymentInfo> PaymentInfo { get; set; }
        public virtual ICollection<RouteProcess> RouteProcess { get; set; }
        public virtual ICollection<MessageUser> MessageUser { get; set; }
        public virtual ICollection<Support> Support { get; set; }
        public virtual ICollection<UserCar> UserCar { get; set; }

    }
}
