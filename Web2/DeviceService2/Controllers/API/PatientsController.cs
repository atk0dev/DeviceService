using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DeviceService2.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Linq;

namespace DeviceService2.Controllers.API
{
    public class PatientsController : ApiController
    {
        private IMongoCollection<Patient> _patients;

        public PatientsController()
        {
            this._patients = PatientDb.Open();
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Return all data from mongo database</returns>
        public async Task<IHttpActionResult> Get()
        {
            var documents = await _patients.Find(new BsonDocument()).ToListAsync();
            if (!documents.Any())
            {
                return NotFound();
            }

            return Ok(documents);

            //var result = new List<Patient>();
            //var filter = new BsonDocument();
            //using (var cursor = await _patients.FindAsync(filter))
            //{
            //    while (await cursor.MoveNextAsync())
            //    {
            //        var batch = cursor.Current;
            //        foreach (var document in batch)
            //        {
            //            result.Add(document);
            //        }
            //    }
            //}

            //return result;
        }

        public async Task<IHttpActionResult> Get(string id)
        {
            var document = await _patients.FindAsync(x => x.Id == id);
            var result = await document.FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Route("api/patients/{id}/medications")]
        public async Task<IHttpActionResult> GetMedications(string id)
        {
            var document = await _patients.FindAsync(x => x.Id == id);
            var m = await document.FirstOrDefaultAsync();
            if (m == null || m.Medications == null || !m.Medications.Any())
            {
                return NotFound();
            }

            return Ok(m.Medications);
        }
    }
}
