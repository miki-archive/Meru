namespace IA.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ReworkMeruDb : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Identifiers");
            AddColumn("dbo.Identifiers", "GuildId", c => c.Long(nullable: false));
            AddColumn("dbo.Identifiers", "IdentifierId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Identifiers", new[] { "GuildId", "IdentifierId" });
            DropColumn("dbo.Identifiers", "guild_id");
        }

        public override void Down()
        {
            AddColumn("dbo.Identifiers", "guild_id", c => c.Long(nullable: false, identity: true));
            DropPrimaryKey("dbo.Identifiers");
            DropColumn("dbo.Identifiers", "IdentifierId");
            DropColumn("dbo.Identifiers", "GuildId");
            AddPrimaryKey("dbo.Identifiers", "guild_id");
        }
    }
}