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
using System.Web.Http.Routing;
using DeviceService2.DataContexts;
using DeviceService2.Entities;
using DeviceService2.Models;
using DeviceService2.Services;

namespace DeviceService2.Controllers.API
{
    //[Authorize]
    public class ValuesController : BaseApiController
    {
        private DevicesDb db = new DevicesDb();
        private ISimpleApiIdentity _apiIdentity;

        const int PAGE_SIZE = 10;

        public ValuesController(ISimpleApiIdentity identity)
        {
            _apiIdentity = identity;
        }

        // GET: api/Values
        [HttpGet]
        public HttpResponseMessage GetValues(int page = 0)
        {
            var dbValues = db.Values.Include(v => v.Device).OrderByDescending(v => v.CreatedAt);

            var totalCount = dbValues.Count();
            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE);

            var helper = new UrlHelper(Request);
            var prevUrl = page > 0 ? helper.Link("DefaultApi", new { page = page - 1 }) : string.Empty;
            var nextUrl = page < totalPages - 1 ? helper.Link("DefaultApi", new { page = page + 1 }) : string.Empty;



            var values = dbValues.Skip(PAGE_SIZE * page)
                .Take(PAGE_SIZE)
                .ToList()
                .Select(v => TheModelFactory.Create(v))
                .AsQueryable();


            return Request.CreateResponse(HttpStatusCode.OK, new 
            {
                TotalCount = totalCount,
                TotalPage = totalPages,
                PrevPageUrl = prevUrl,
                NextPageUrl = nextUrl,
                Values = values
            });
        }

        // GET: api/Values/5
        [HttpGet]
        [ResponseType(typeof(ValueDetailDTO))]
        public async Task<IHttpActionResult> GetValue(int id)
        {
            var dbValue = await db.Values.Include(v => v.Device)
                .SingleOrDefaultAsync(b => b.Id == id);
            var value = TheModelFactory.CreateValueDetail(dbValue);

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

            var dto = TheModelFactory.Create(value);

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
