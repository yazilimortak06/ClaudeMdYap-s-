using AutoMapper;
using Shared.Domain.Dto.ApiDto.PanelAdminDtos;
using Shared.Domain.Dto.ApiDto.PanelRootAdminDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Dto.ApiDto.AuthenticationDtos
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LoginFormKey { get; set; }
    }
    public class LoginRequestProfile : Profile
    {
        public LoginRequestProfile()
        {
            CreateMap<LoginRequestDto, PanelAdminFilterDto>();
            CreateMap<LoginRequestDto, PanelRootAdminFilterDto>();
        }
    }
}
