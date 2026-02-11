namespace CSService.Queries.Impl;

internal sealed partial class ExaminerQueries
{
    private const string GET_SQL = "select * from examiners where examiners.login = @Login";
    private const string CREATE_SQL = @"insert into examiners
                                        (create_date, update_date, first_name, last_name, login, password, password_salt)
                                        values(@CreateDate, @UpdateDate, @FirstName, @LastName, @Login, @Password, @PasswordSalt)";
}
