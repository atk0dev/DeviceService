using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeviceService2.Entities;

namespace DeviceService2.Models
{
    public class ModelFactory
    {
        public ValueDTO Create(Value value)
        {
            return new ValueDTO()
            {
                Id = value.Id,
                Title = value.Title,
                DeviceName = value.Device.Title,
                Date = value.CreatedAt
            };
        }
    }
}