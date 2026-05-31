using FluentMigrator;

namespace CSService.Migrations.Year2024;

[Migration(202602111700, "Add scene_photos table for multiple photos per scene")]
public sealed class Migration_202602111700 : Migration
{
    public override void Up() {
        Create.Table("scene_photos")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("scene_id").AsInt64().NotNullable().ForeignKey("scenes", "id")
            .WithColumn("filename").AsString(256).NotNullable()
            .WithColumn("order_index").AsInt32().NotNullable().WithDefaultValue(0);

        // Migrate existing scenes: create scene_photos from scenes.filename
        Execute.Sql(@"
            INSERT INTO scene_photos (create_date, update_date, scene_id, filename, order_index)
            SELECT create_date, update_date, id, filename, 0
            FROM scenes
            WHERE filename IS NOT NULL AND filename != ''
        ");
    }

    public override void Down() {
        Delete.Table("scene_photos");
    }
}
