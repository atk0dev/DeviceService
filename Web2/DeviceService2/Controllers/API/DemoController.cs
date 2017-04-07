using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DeviceService2.DataContexts.Demo;

namespace DeviceService2.Controllers.API
{
    public class DemoController : ApiController
    {
        private IRepository _repository;

        public DemoController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public object GetAll()
        {
            var data = _repository.GetAll();
            return data;
        }


    }
}
