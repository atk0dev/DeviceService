using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceService.Models
{
    public class Device
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}