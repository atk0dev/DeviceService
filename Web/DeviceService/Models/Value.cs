using System;
using System.ComponentModel.DataAnnotations;

namespace DeviceService.Models
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