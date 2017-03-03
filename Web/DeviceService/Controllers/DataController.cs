using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DeviceService.Models;

namespace DeviceService.Controllers
{
    public class DataController : ApiController
    {
        private DeviceServiceContext db = new DeviceServiceContext();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ReceiveData(ReceiveDataDto data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(data.Title1) && data.Value1 > 0)
            {
                var value = new Value
                {
                    DeviceId = data.DeviceId,
                    Data = data.Value1,
                    Title = data.Title1
                };

                db.Values.Add(value);
            }

            if (!string.IsNullOrEmpty(data.Title2) && data.Value2 > 0)
            {
                var value = new Value
                {
                    DeviceId = data.DeviceId,
                    Data = data.Value2,
                    Title = data.Title2
                };

                db.Values.Add(value);
            }

            if (!string.IsNullOrEmpty(data.Title3) && data.Value3 > 0)
            {
                var value = new Value
                {
                    DeviceId = data.DeviceId,
                    Data = data.Value3,
                    Title = data.Title3
                };

                db.Values.Add(value);
            }

            await db.SaveChangesAsync();

            return Ok("Data has been received");
        }
    }
}
