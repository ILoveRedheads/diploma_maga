namespace CSService.Queries.Impl;

internal sealed partial class SessionQueries
{
    private const string CHECK_SCENE_SQL = "select id from scenes where id = @Id";
    private const string CHECK_SESSION_SQL = "select id from sessions where id = @Id";
    private const string GET_PHOTOS_SQL = "select * from photos where photos.session_id = @Id";
    private const string GET_COMMENTS_SQL = "select * from comments where comments.session_id = @Id";
    private const string GET_COMMENT_SQL = "select * from comments where comments.id = @Id";
    private const string GET_PHOTO_SQL = "select * from photos where photos.id = @Id";
    private const string DELETE_SQL = @"delete from photos where session_id = @Id;
                                        delete from comments where session_id = @Id;
                                        delete from sessions where id = @Id";

    private const string CREATE_SQL = @"insert into
                                        sessions(create_date, update_date, scene_id, vr_headset_id, first_name, last_name, group_name)
                                        values(@CreateDate, @UpdateDate, @SceneId, @VrHeadsetId, @FirstName, @LastName, @GroupName)
                                        returning id";

    private const string GET_BY_ID_SQL = @"select
                                               sessions.id,
                                               sessions.first_name,
                                               sessions.last_name,
                                               sessions.group_name,
                                               vr_headsets.name as vr_headsets_name,
                                               scenes.name as scene_name
                                           from sessions
                                           left join vr_headsets on sessions.vr_headset_id = vr_headsets.id
                                           left join scenes on sessions.scene_id = scenes.id
                                           where sessions.id = @Id";

    private const string CREATE_COMMENT_SQL = @"insert into
                                                comments(create_date, update_date, session_id, audio_filename, text_filename)
                                                values(@CreateDate, @UpdateDate, @SessionId, @AudioFilename, @TextFilename)
                                                returning id";

    private const string CREATE_PHOTO_SQL = @"insert into
                                                photos(create_date, update_date, session_id, audio_filename, screenshot_filename, text_filename)
                                                values(@CreateDate, @UpdateDate, @SessionId, @AudioFilename, @ScreenshotFilename, @TextFilename)
                                                returning id";

    private const string GET_PAGE_WHERE_SQL = @"select *
                                            from sessions
                                            left join scenes on scenes.id = sessions.scene_id
                                            left join vr_headsets on vr_headsets.id = sessions.vr_headset_id
                                            where first_name like @Search
                                                or last_name like @Search
                                                or group_name like @Search
                                            limit @Limit offset @Offset";

    private const string GET_PAGE_SQL = @"select *
                                            from sessions
                                            left join scenes on scenes.id = sessions.scene_id
                                            left join vr_headsets on vr_headsets.id = sessions.vr_headset_id
                                            limit @Limit offset @Offset";

    private const string COUNT_WHERE_SQL = @"select count(*)
                                            from sessions
                                            where first_name like @Search
                                                or last_name like @Search
                                                or group_name like @Search";

    private const string COUNT_SQL = @"select count(*) from sessions";
}
