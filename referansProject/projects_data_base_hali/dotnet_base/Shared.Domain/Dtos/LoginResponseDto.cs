using AutoMapper;
using Shared.Domain.Dto.TockenDto.JwtTockenDtos;
using Shared.Domain.Entities.ApiEntities.AdminModule;
using Shared.Domain.Enums.ApiEnums;
using Shared.Domain.Enums.TockenEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Dto.ApiDto.AuthenticationDtos
{
    public class LoginResponseDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string CompanyName { get; set; }
        public string FirmName { get; set; }
        public string ConnectionId { get; set; }
        public AdminManagmentTypeEnum AdminManagmentType { get; set; }
        public AccessTokenDto AccessToken { get; set; }
#nullable enable
        public long? CompanyId { get; set; }
        public long? FirmId { get; set; }
#nullable disable
    }
    public class LoginResponseProfile : Profile
    {
        public LoginResponseProfile()
        {
            CreateMap<PanelAdmin, LoginResponseDto>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                                                    .ForMember(dest => dest.FirmName, opt => opt.MapFrom(src => src.Firm.Name));
            CreateMap<PanelRootAdmin, LoginResponseDto>();
        }
    }
}
