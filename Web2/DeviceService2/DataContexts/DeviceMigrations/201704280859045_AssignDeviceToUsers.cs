namespace DeviceService2.DataContexts.DeviceMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignDeviceToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Code", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Devices", "Users", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "Users");
            DropColumn("dbo.Devices", "Code");
        }
    }
}
