using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService2.Services
{
    public interface ISimpleApiIdentity
    {
        string CurrentUser { get; }
    }
}