using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceService2.Services
{
    public class SimpleApiIdentity : ISimpleApiIdentity
    {
        public string CurrentUser {
            get { return "ApiUser"; }
        }
    }
}