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

            var value = new Value
            {
                DeviceId = data.DeviceId,
                Data = data.Value,
                Title = data.Title
            };

            db.Values.Add(value);
            await db.SaveChangesAsync();

            return Ok("Data has been received");
        }
    }
}
