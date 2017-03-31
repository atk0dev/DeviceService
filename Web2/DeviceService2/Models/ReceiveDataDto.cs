using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService2.Models
{
    public class ReceiveDataDto
    {
        public int DeviceId { get; set; }

        public string Title1 { get; set; }

        public decimal Value1 { get; set; }

        public string Title2 { get; set; }

        public decimal Value2 { get; set; }

        public string Title3 { get; set; }

        public decimal Value3 { get; set; }
    }
}