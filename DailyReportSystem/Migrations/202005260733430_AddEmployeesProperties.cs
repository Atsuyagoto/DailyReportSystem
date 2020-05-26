namespace DailyReportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeesProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EmployeeName", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.AspNetUsers", "CreatedAt", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "UpdatedAt", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "DeleteFlg", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DeleteFlg");
            DropColumn("dbo.AspNetUsers", "UpdatedAt");
            DropColumn("dbo.AspNetUsers", "CreatedAt");
            DropColumn("dbo.AspNetUsers", "EmployeeName");
        }
    }
}
