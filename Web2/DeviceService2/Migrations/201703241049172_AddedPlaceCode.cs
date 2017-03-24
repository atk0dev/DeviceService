namespace DeviceService2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPlaceCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PlaceCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "PlaceCode");
        }
    }
}
