namespace Meru.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommandStates",
                c => new
                    {
                        CommandName = c.String(nullable: false, maxLength: 128),
                        ChannelId = c.Long(nullable: false),
                        State = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.CommandName, t.ChannelId });
            
            CreateTable(
                "dbo.Identifiers",
                c => new
                    {
                        guild_id = c.Long(nullable: false),
                        identifier = c.String(),
                    })
                .PrimaryKey(t => t.guild_id);
            
            CreateTable(
                "dbo.ModuleStates",
                c => new
                    {
                        ModuleName = c.String(nullable: false, maxLength: 128),
                        ChannelId = c.Long(nullable: false),
                        State = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.ModuleName, t.ChannelId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ModuleStates");
            DropTable("dbo.Identifiers");
            DropTable("dbo.CommandStates");
        }
    }
}
