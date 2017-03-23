namespace DeviceService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateToValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Values", "CreatedAt", c => c.DateTime(nullable: false, defaultValue: DateTime.Now));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Values", "CreatedAt");
        }
    }
}
