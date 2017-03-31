using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DeviceService2.Startup))]
namespace DeviceService2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
