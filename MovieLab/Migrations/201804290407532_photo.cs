namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class photo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "Photo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "Photo");
        }
    }
}
