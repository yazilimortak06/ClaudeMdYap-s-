// Kaynak: E:\Projeler\Backend\RotaWattBackEnd\Framework\Core\FrameworkCore\Bases\BaseServices\BaseService.cs

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkCore.Bases.BaseServices
{
    /// <summary>
    /// Tüm servis sınıflarının türediği temel sınıf.
    /// AutoMapper instance'ını inject eder ve alt sınıflara koruyarak sunar.
    /// </summary>
    public class BaseService
    {
        protected IMapper _mapper;

        public BaseService(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
