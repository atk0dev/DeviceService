using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DeviceService2.Counters;
using Microsoft.AspNet.SignalR;

namespace DeviceService2.Hubs
{
    public class PerfHub : Hub
    {
        public PerfHub()
        {
            StartCounterCollection();
        }

        private void StartCounterCollection()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var perfService = new PerfCounterService();
                while (true)
                {
                    var results = perfService.GetResults();
                    Clients.All.newCounters(results);
                    await Task.Delay(2000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Send(string message)
        {
            string userName = string.Empty;
            if (Context.User != null && Context.User.Identity != null)
            {
                userName = Context.User.Identity.Name; 
                
            }
            
            Clients.All.newMessage($"{userName} - {message}");
        }
    }
}