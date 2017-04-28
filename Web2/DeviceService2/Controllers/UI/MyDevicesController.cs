using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DeviceService2.DataContexts;
using DeviceService2.Models;

namespace DeviceService2.Controllers.UI
{
    [Authorize]
    public class MyDevicesController : Controller
    {
        private DevicesDb db = new DevicesDb();


        // GET: MyDevices
        public async Task<ActionResult> Index()
        {
            var userName = User.Identity.Name;
            var deviceslist = await db.Devices.Where(d => d.Users.Contains(userName)).ToListAsync();

            var viewList = new MyDevicesViewModel();
            viewList.Data = new List<MyDeviceViewModel>();

            foreach (var device in deviceslist)
            {
                //// check device is assigned to me
                var assignedUsersList = device.Users.Split(',');
                if (assignedUsersList.Length != 0)
                {
                    if (assignedUsersList.Contains(userName))
                    {
                        var 
                        myDevice = new MyDeviceViewModel
                        {
                            DeviceId = device.Id,
                            DeviceCode = device.Code,
                            DeviceTitle = device.Title
                        };

                        var value = await db.Values.FirstOrDefaultAsync(v => v.DeviceId == device.Id);
                        if (value != null)
                        {
                            myDevice.LatestValue = value.Data;
                            myDevice.LatestValueAt = value.CreatedAt;
                        }

                        viewList.Data.Add(myDevice);
                    }
                }
            }

            return View(viewList);
        }
    }
}