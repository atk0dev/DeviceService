using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using DeviceService2.Entities;

namespace DeviceService2.Models
{
    public class ModelFactory
    {
        private UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }

        public ValueDTO Create(Value value)
        {
            var result = new ValueDTO()
            {
                Id = value.Id,
                Title = value.Title,
                DeviceName = string.Empty,
                Date = value.CreatedAt,
                Url = _urlHelper.Link("DefaultApi", new { id = value.Id })
            };

            if (value.Device != null)
            {
                result.DeviceName = value.Device.Title;
            }

            return result;
        }

        public ValueDetailDTO CreateValueDetail(Value value)
        {
            var result = new ValueDetailDTO()
            {
                Id = value.Id,
                Title = value.Title,
                Data = value.Data,
                DeviceName = string.Empty,
                Date = value.CreatedAt,
                Url = _urlHelper.Link("DefaultApi", new { id = value.Id })
            };

            if (value.Device != null)
            {
                result.DeviceName = value.Device.Title;
            }

            return result;
        }
    }
}