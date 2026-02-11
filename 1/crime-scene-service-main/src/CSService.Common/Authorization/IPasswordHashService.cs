namespace CSService.Common.Authorization;

public interface IPasswordHashService
{
    HashedPassword Hash(string password);

    bool Verify(HashedPassword hashedPassword, string password);
}
