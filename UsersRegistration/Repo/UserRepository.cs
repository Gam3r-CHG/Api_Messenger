using System.Security.Cryptography;
using System.Text;
using UsersRegistration.Db;
using UsersRegistration.Models;

namespace UsersRegistration.Repo;

public class UserRepository : IUserRepository
{
    private readonly UserContext _context;

    public UserRepository(UserContext context)
    {
        _context = context;
    }

    public void AddUser(string email, string password, RoleId roleId)
    {
        if (roleId == RoleId.Admin)
        {
            var c = _context.Users.Count(x => x.RoleId == RoleId.Admin);

            if (c > 0)
            {
                throw new Exception("Admin can only be one");
            }
        }

        if (_context.Users.Any(x => x.Email.ToLower().Equals(email.ToLower())))
        {
            throw new Exception("User already exists");
        }

        var user = new User();
        user.Email = email;
        user.RoleId = roleId;

        user.Salt = new byte[16];
        new Random().NextBytes(user.Salt);

        var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();

        SHA512 sha512 = new SHA512Managed();
        user.Password = sha512.ComputeHash(data);

        _context.Add(user);

        _context.SaveChanges();
    }

    public Guid DeleteUser(string email)
    {
        var user = _context.Users.FirstOrDefault(
            x => x.RoleId == RoleId.User && x.Email.ToLower().Equals(email.ToLower()));

        if (user == null)
        {
            throw new Exception("User not found");
        }

        _context.Remove(user);
        _context.SaveChanges();
        return user.Id;
    }

    public RoleId CheckUser(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));

        if (user == null)
        {
            throw new Exception("User not found");
        }

        var data = Encoding.ASCII.GetBytes(password).Concat(user.Salt).ToArray();
        SHA512 sha512 = new SHA512Managed();
        var bpassword = sha512.ComputeHash(data);

        if (user.Password.SequenceEqual(bpassword))
        {
            return user.RoleId;
        }

        throw new Exception("Wrong password");
    }

    public Guid GetUserId(string email)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));

        if (user is null)
        {
            throw new Exception("User not found");
        }

        return user.Id;
    }

    public Guid GetUserId(UserModel user)
    {
        var userDb = _context.Users
            .FirstOrDefault(x => x.Email.ToLower().Equals(user.Email.ToLower()));

        if (userDb is null)
        {
            throw new Exception("User not found");
        }

        return userDb.Id;
    }

    public IEnumerable<string> GetUsers()
    {
        var users = _context.Users
            .Where(x => x.RoleId == RoleId.User)
            .Select(x => x.Email).ToList();

        return users;
    }
}