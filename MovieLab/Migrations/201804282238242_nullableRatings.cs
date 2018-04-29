namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullableRatings : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reviews", "Ratings", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reviews", "Ratings", c => c.Int(nullable: false));
        }
    }
}
