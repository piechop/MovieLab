namespace MovieLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ra : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    UserID = c.Int(nullable: false),
                    Rating = c.Byte(nullable: false),
                })
                .PrimaryKey(t => t.ID);
        }
        
        public override void Down()
        {
        }
    }
}
