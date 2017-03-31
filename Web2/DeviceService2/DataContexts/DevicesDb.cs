using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using DeviceService2.Entities;

namespace DeviceService2.DataContexts
{
    public class DevicesDb : DbContext
    {
        public DevicesDb() : base("DefaultConnection")
        {
            //Database.Log = sql => Debug.Write(sql);

            Database.Log = Log;
        }

        private void Log(string sql)
        {
            Debug.Write(sql);
        }

        public DbSet<Device> Devices { get; set; }

        public DbSet<Value> Values { get; set; }
    }
}