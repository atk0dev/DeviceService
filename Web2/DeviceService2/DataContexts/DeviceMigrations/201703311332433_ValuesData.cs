namespace DeviceService2.DataContexts.DeviceMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValuesData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Values",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Data = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeviceId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Devices", t => t.DeviceId, cascadeDelete: true)
                .Index(t => t.DeviceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Values", "DeviceId", "dbo.Devices");
            DropIndex("dbo.Values", new[] { "DeviceId" });
            DropTable("dbo.Values");
        }
    }
}
