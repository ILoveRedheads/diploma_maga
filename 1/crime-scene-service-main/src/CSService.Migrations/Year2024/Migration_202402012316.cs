using FluentMigrator;

namespace CSService.Migrations.Year2024;

[Migration(202402012316, "Initial")]
public sealed class Migration_202402012316 : Migration
{
    public override void Up() {
        Create.Table("scenes")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("name").AsString(128).NotNullable()
            .WithColumn("filename").AsString(256).NotNullable();

        Create.Table("sessions")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("scene_id").AsInt64().NotNullable().ForeignKey("scenes", "id")
            .WithColumn("vr_headset_id").AsInt64().NotNullable().ForeignKey("vr_headsets", "id")
            .WithColumn("first_name").AsString(128).NotNullable()
            .WithColumn("last_name").AsString(128).NotNullable()
            .WithColumn("group_name").AsString(64).NotNullable();

        Create.Table("photos")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("session_id").AsInt64().NotNullable().ForeignKey("sessions", "id")
            .WithColumn("audio_filename").AsString(256).NotNullable()
            .WithColumn("screenshot_filename").AsString(256).NotNullable()
            .WithColumn("text_filename").AsString(256).NotNullable();

        Create.Table("comments")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("session_id").AsInt64().NotNullable().ForeignKey("sessions", "id")
            .WithColumn("audio_filename").AsString(256).NotNullable()
            .WithColumn("text_filename").AsString(256).NotNullable();

        Create.Table("vr_headsets")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("name").AsString(64).NotNullable()
            .WithColumn("ip").AsString(15).Nullable()
            .WithColumn("mac_address").AsString(17).NotNullable()
            .WithColumn("scene_id").AsInt64().Nullable().ForeignKey("scenes", "id");

        Create.Table("examiners")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("create_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("update_date").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("first_name").AsString(128).NotNullable()
            .WithColumn("last_name").AsString(128).NotNullable()
            .WithColumn("login").AsString(128).NotNullable()
            .WithColumn("password").AsString(256).NotNullable()
            .WithColumn("password_salt").AsString(256).NotNullable();
    }

    public override void Down() {
        Delete.Table("examiners");
        Delete.Table("vr_headsets");
        Delete.Table("comments");
        Delete.Table("scenes");
    }
}
