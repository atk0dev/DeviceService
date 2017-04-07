using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService2.DataContexts.Demo
{
    public class Repository : IRepository
    {
        private IDatabase _database;

        public Repository(IDatabase database)
        {
            _database = database;
        }

        public object GetAll()
        {
            return _database.GetAll();
        }
    }
}