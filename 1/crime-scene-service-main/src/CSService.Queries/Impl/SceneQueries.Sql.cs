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
}
