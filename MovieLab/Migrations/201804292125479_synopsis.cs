namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class synopsis : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "Synopsis", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "Synopsis");
        }
    }
}
