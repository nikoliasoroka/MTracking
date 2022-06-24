using System.ComponentModel.DataAnnotations;
using MTracking.Core.Enums;

namespace MTracking.BLL.DTOs.Device.Request
{
    public class DeviceDto
    {
        [Required]
        public string FirebaseToken { get; set; }

        [Required]
        [Range(0, 1)]
        public ApplicationType ApplicationType { get; set; }
    }
}
