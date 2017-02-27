using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DeviceService.Models;

namespace DeviceService.Controllers
{
    public class ValuesController : ApiController
    {
        private DeviceServiceContext db = new DeviceServiceContext();

        // GET: api/Values
        [HttpGet]
        public IQueryable<ValueDTO> GetValues()
        {
            var values = from v in db.Values
                        select new ValueDTO()
                        {
                            Id = v.Id,
                            Title = v.Title,
                            DeviceName = v.Device.Name
                        };

            return values;
        }

        // GET: api/Values/5
        [HttpGet]
        [ResponseType(typeof(ValueDetailDTO))]
        public async Task<IHttpActionResult> GetValue(int id)
        {
            var value = await db.Values.Include(v => v.Device).Select(b =>
                new ValueDetailDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Data = b.Data,
                    DeviceName = b.Device.Name
                }).SingleOrDefaultAsync(b => b.Id == id);
            if (value == null)
            {
                return NotFound();
            }

            return Ok(value);
        }

        // PUT: api/Values/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutValue(int id, Value value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != value.Id)
            {
                return BadRequest();
            }

            db.Entry(value).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Values
        [HttpPost]
        [ResponseType(typeof(Value))]
        public async Task<IHttpActionResult> PostValue(Value value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Values.Add(value);
            await db.SaveChangesAsync();

            // Load device name
            db.Entry(value).Reference(x => x.Device).Load();

            var dto = new ValueDTO()
            {
                Id = value.Id,
                Title = value.Title,
                DeviceName = value.Device.Name
            };

            return CreatedAtRoute("DefaultApi", new { id = value.Id }, dto);
        }

        // DELETE: api/Values/5
        [HttpDelete]
        [ResponseType(typeof(Value))]
        public async Task<IHttpActionResult> DeleteValue(int id)
        {
            Value value = await db.Values.FindAsync(id);
            if (value == null)
            {
                return NotFound();
            }

            db.Values.Remove(value);
            await db.SaveChangesAsync();

            return Ok(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ValueExists(int id)
        {
            return db.Values.Count(e => e.Id == id) > 0;
        }
    }
}