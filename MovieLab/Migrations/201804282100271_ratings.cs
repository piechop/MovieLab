namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "Ratings", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reviews", "Ratings");
        }
    }
}
