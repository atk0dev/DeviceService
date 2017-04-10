
using System;

namespace DeviceService2.Models
{
    public class ValueDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DeviceName { get; set; }

        public DateTime Date { get; set; }

        public string Url { get; set; }
    }
}