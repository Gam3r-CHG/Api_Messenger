using UsersRegistration.Db;
using UsersRegistration.Models;

namespace UsersRegistration.Repo;

public interface IUserRepository
{
    public void AddUser(string email, string password, RoleId roleId);
    public Guid DeleteUser(string email);
    public RoleId CheckUser(string email, string password);
    public Guid GetUserId(string email);
    public Guid GetUserId(UserModel user);
    public IEnumerable<string> GetUsers();
}