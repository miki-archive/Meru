namespace IA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReworkPrefix : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Identifiers");
            DropColumn("dbo.Identifiers", "guild_id");
            Sql("truncate table Identifiers");
            AddColumn("dbo.Identifiers", "GuildId", c => c.Long(nullable: false, identity: false));
            AddColumn("dbo.Identifiers", "IdentifierId", c => c.String(nullable: false, maxLength: 128, defaultValue: ">"));
            AddPrimaryKey("dbo.Identifiers", new[] { "GuildId", "IdentifierId" });
        }
        
        public override void Down()
        {
            // that bad! // really bad
            AddColumn("dbo.Identifiers", "guild_id", c => c.Long(nullable: false, identity: true));
            DropPrimaryKey("dbo.Identifiers");
            DropColumn("dbo.Identifiers", "IdentifierId");
            DropColumn("dbo.Identifiers", "GuildId");
            AddPrimaryKey("dbo.Identifiers", "guild_id");
        }
    }
}
