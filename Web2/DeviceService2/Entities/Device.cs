using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeviceService2.Entities
{
    public class Device
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Code { get; set; }

        public DeviceType DeviceType { get; set; }

        public string Users { get; set; }
    }
}