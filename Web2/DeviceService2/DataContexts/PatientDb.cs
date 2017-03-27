using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeviceService2.Models;
using MongoDB.Driver;

namespace DeviceService2.DataContexts
{
    public static class PatientDb
    {
        public static IMongoCollection<Patient> Open()
        {
            var client = new MongoClient("mongodb://localhost");
            var db = client.GetDatabase("PatientDb");
            var data = db.GetCollection<Patient>("Patients");
            return data;
        }
    }
}