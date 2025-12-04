namespace dacn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NganSaches", "DaChi", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NganSaches", "ConLai", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NganSaches", "ConLai");
            DropColumn("dbo.NganSaches", "DaChi");
        }
    }
}
