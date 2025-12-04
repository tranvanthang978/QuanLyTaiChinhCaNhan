namespace dacn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChiTietModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.NganSaches", "DaChi");
            DropColumn("dbo.NganSaches", "ConLai");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NganSaches", "ConLai", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.NganSaches", "DaChi", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
