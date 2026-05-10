// KAYNAK: E:\Projeler\Backend\PixdinnPerdeci\PixdinnPerdeci\Models\ErrorViewModel.cs

namespace PixdinnPerdeci.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
