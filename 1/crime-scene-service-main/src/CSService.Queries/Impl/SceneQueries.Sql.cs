namespace CSService.Queries.Impl;

internal sealed partial class SceneQueries
{
    private const string GET_BY_NAME_SQL = "select * from scenes where name = @name";
    private const string GET_BY_ID_SQL = "select * from scenes where id = @Id";
    private const string GET_PAGE_SQL = "select * from scenes limit @Limit offset @Offset";
    private const string COUNT_SQL = "select count(*) from scenes";
    private const string CREATE_SQL = @"insert into scenes(create_date, update_date, name, filename)
                                        values(@CreateDate, @UpdateDate, @Name, @Filename)
                                        returning id";

    private const string CREATE_SCENE_PHOTO_SQL = @"insert into scene_photos(create_date, update_date, scene_id, filename, order_index)
                                                    values(@CreateDate, @UpdateDate, @SceneId, @Filename, @OrderIndex)
                                                    returning id";

    private const string GET_SCENE_PHOTOS_SQL = "select * from scene_photos where scene_id = @SceneId order by order_index";

    private const string GET_SCENE_PHOTO_BY_ID_SQL = "select * from scene_photos where id = @Id";

    private const string GET_SCENE_PHOTO_BY_INDEX_SQL = @"select sp.* from scene_photos sp
                                                          inner join vr_headsets vh on vh.scene_id = sp.scene_id
                                                          where vh.mac_address = @MacAddress
                                                          and sp.order_index = @OrderIndex";

    private const string COUNT_SCENE_PHOTOS_SQL = "select count(*) from scene_photos where scene_id = @SceneId";

    private const string GET_MAX_ORDER_INDEX_SQL = "select coalesce(max(order_index), -1) from scene_photos where scene_id = @SceneId";

    private const string GET_SCENE_META_SQL = @"select s.id as SceneId, s.name as SceneName,
                                                (select count(*) from scene_photos sp where sp.scene_id = s.id) as PhotoCount
                                                from scenes s
                                                inner join vr_headsets vh on vh.scene_id = s.id
                                                where vh.mac_address = @MacAddress";
}
