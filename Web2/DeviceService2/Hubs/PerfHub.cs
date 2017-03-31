using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DeviceService2.Hubs
{
    public class PerfHub : Hub
    {
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