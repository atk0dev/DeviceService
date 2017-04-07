using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DeviceService2.DataContexts;
using DeviceService2.Entities;
using DeviceService2.Models;

namespace DeviceService2.Controllers.API
{
    //[Authorize]
    public class ValuesController : ApiController
    {
        private DevicesDb db = new DevicesDb();

        ModelFactory _modelFactory;

        public ValuesController()
        {
            _modelFactory = new ModelFactory();
        }

        // GET: api/Values
        [HttpGet]
        public IQueryable<ValueDTO> GetValues()
        {
            var dbValues = db.Values.Include(v => v.Device).ToList();
            var values = dbValues.Select(v => _modelFactory.Create(v)).OrderByDescending(v => v.Date).AsQueryable();

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
                    DeviceName = b.Device.Title,
                    Date = b.CreatedAt
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

            value.CreatedAt = DateTime.Now;
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

            value.CreatedAt = DateTime.Now;
            db.Values.Add(value);
            await db.SaveChangesAsync();

            // Load device name
            db.Entry(value).Reference(x => x.Device).Load();

            var dto = new ValueDTO()
            {
                Id = value.Id,
                Title = value.Title,
                DeviceName = value.Device.Title,
                Date = value.CreatedAt
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
