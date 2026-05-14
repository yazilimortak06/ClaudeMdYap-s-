// KAYNAK: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Bases\Models\ApiOptions.cs

using System.Reflection;

namespace PixdinnPerdeci.Bases.Models
{
    public class ApiOptions
    {
        public IEnumerable<Assembly> RegistrationAssemblies { get; set; }
        public IEnumerable<Type> HubListSignalR { get; set; }
        public IEnumerable<Type> ApiClientList { get; set; }

        public string ApiName { get; set; }
    }
}
