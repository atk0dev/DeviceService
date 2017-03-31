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
            //var client = new MongoClient("mongodb://localhost");
            var client = new MongoClient("mongodb://user1:POIq1w2e3r4@ds145750.mlab.com:45750/patientdb");
            var db = client.GetDatabase("patientdb");
            var data = db.GetCollection<Patient>("Patients");
            return data;
        }
    }
}