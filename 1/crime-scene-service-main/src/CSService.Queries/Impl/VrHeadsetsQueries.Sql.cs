namespace CSService.Queries.Impl;

internal sealed partial class VrHeadsetsQueries
{
    private const string GET_SQL = @"select * from vr_headsets as vh
                                        where vh.name = @Name or vh.mac_address = @MacAddress";

    private const string GET_SAME_SQL = @"select * from vr_headsets
                                        where id != @Id and (name = @Name or mac_address = @MacAddress)";

    private const string GET_ALL_SQL = @"select * from vr_headsets as vh left join scenes on scenes.id = vh.scene_id";
    private const string GET_BY_ID_SQL = "select * from vr_headsets as vh where vh.id = @id";
    private const string CREATE_SQL = @"insert into vr_headsets
                                        (create_date, update_date, name, mac_address)
                                        values(@CreateDate, @UpdateDate, @Name, @MacAddress)";

    private const string UPDATE_SQL = @"update vr_headsets
                                        set update_date = @UpdateDate, name = @Name, mac_address = @MacAddress, scene_id = @SceneId
                                        where id = @Id";
}
