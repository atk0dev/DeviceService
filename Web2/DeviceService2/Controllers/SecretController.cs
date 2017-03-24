using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeviceService2.Controllers
{
    [Authorize]
    //[Authorize(Users = "atk, qwe")]
    //[Authorize(Roles = "admin, monitor")]
    public class SecretController : Controller
    {
        // GET: Secret
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ContentResult Overt()
        {
            return Content("This is not a secret");
        }
    }
}