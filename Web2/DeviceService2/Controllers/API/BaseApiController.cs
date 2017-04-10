using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DeviceService2.Models;

namespace DeviceService2.Controllers.API
{
    public abstract class BaseApiController : ApiController
    {
        ModelFactory _modelFactory;

        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request);
                }
                return _modelFactory;
            }
        }
    }
}