using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService2.Models
{
    public class MyDeviceViewModel
    {
        public int DeviceId { get; set; }
        public string DeviceTitle { get; set; }

        public string DeviceCode { get; set; }

        public decimal LatestValue { get; set; }

        public DateTime LatestValueAt { get; set; }
    }
}