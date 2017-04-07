using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService2.DataContexts.Demo
{
    public interface IRepository
    {
        object GetAll();
    }
}