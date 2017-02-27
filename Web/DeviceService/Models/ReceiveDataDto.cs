using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService.Models
{
    public class ReceiveDataDto
    {
        public int DeviceId { get; set; }

        public string Title { get; set; }

        public decimal Value { get; set; }
            
    }
}