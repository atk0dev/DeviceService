using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeviceService2.Models;
using MongoDB.Driver;

namespace DeviceService2
{
    public static class MongoConfig
    {
        public static void Seed()
        {
            var patients = PatientDb.Open();
            if (!patients.AsQueryable().Any(p => p.Name == "test"))
            {
                var list = new List<Patient>
                {
                    new Patient
                    {
                        Name = "test1",
                        Ailments = new List<Ailment>
                        {
                            new Ailment { Name = "A1" },
                            new Ailment { Name = "A2" }
                        },
                        Medications = new List<Medication>
                        {
                            new Medication { Name = "M1", Doses = 1 },
                            new Medication { Name = "M2", Doses = 2 }
                        }
                    },
                    new Patient
                    {
                        Name = "test2",
                        Ailments = new List<Ailment>
                        {
                            new Ailment { Name = "AA1" },
                            new Ailment { Name = "AA2" }
                        },
                        Medications = new List<Medication>
                        {
                            new Medication { Name = "MM1", Doses = 1 },
                            new Medication { Name = "MM2", Doses = 2 }
                        }
                    }
                };
                patients.InsertMany(list);
            }
        }
    }
}