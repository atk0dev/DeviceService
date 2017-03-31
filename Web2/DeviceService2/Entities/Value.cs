using System;
using System.ComponentModel.DataAnnotations;
using DeviceService2.Entities;

namespace DeviceService2.Entities
{
    public class Value
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public decimal Data { get; set; }
        
        // Foreign Key
        public int DeviceId { get; set; }
        // Navigation property
        public Device Device { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}