
using System;

namespace DeviceService2.Models
{
    public class ValueDetailDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Data { get; set; }
        public string DeviceName { get; set; }

        public DateTime Date { get; set; }

        public string Url { get; set; }
    }
}