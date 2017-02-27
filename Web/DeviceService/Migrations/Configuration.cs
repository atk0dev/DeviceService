using DeviceService.Models;

namespace DeviceService.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using DeviceService.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DeviceServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DeviceServiceContext context)
        {
            context.Devices.AddOrUpdate(x => x.Id,
                new Device() { Id = 1, Name = "Device 1" },
                new Device() { Id = 2, Name = "Device 2" },
                new Device() { Id = 3, Name = "Device 3" }
                );

            context.Values.AddOrUpdate(x => x.Id,
                new Value()
                {
                    Id = 1,
                    Title = "Temp",
                    Data = 41,
                    DeviceId = 1
                },
                new Value()
                {
                    Id = 2,
                    Title = "Temp",
                    Data = 31,
                    DeviceId = 1
                });
        }
    }
}
