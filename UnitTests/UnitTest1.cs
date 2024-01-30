using AutoMapper;
using UsersMessages.Db;
using UsersMessages.Dto;
using UsersMessages.Mapper;
using UsersRegistration.Db;
using UsersRegistration.Models;
using UsersRegistration.Repo;
using UsersRegistration.Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace UnitTests;

public class UnitTest1
{
    // For tests
    private readonly string _adminEmail = "admin";
    private readonly string _adminPassword = "pass";

    private readonly string _userDbConnectionString =
        "Host=localhost;Username=postgres;Password=pass;Database=UserLogin";

    private readonly string _messageDbConnectionString =
        "Host=localhost;Username=postgres;Password=pass;Database=UsersMessages";

    [Fact]
    public void TestValidatorLoginModel()
    {
        var errorLoginModel = new LoginModel {Email = "test", Password = "123"};
        var okLoginModel = new LoginModel {Email = "test@mail.ru", Password = "Aaa123!"};

        Validator validator = new Validator();

        var result = validator.ValidateLoginModel(errorLoginModel);
        Assert.False(result.Valid);

        var result2 = validator.ValidateLoginModel(okLoginModel);
        Assert.True(result2.Valid);
    }

    [Fact]
    public void TestUserRepository()
    {
        ITestOutputHelper testOutputHelper = new TestOutputHelper();
        UserContext context = new UserContext(_userDbConnectionString);
        UserRepository userRepository = new UserRepository(context);

        // Test for adding user
        userRepository.AddUser("test1@mail.ru", "Aa1!", RoleId.User);
        var createdGuid = userRepository.GetUserId("test1@mail.ru");
        Assert.IsType<Guid>(createdGuid);

        // Role and login test
        var roleId = userRepository.CheckUser("test1@mail.ru", "Aa1!");
        Assert.Equivalent(roleId, RoleId.User);

        // Test for duplicate user
        Assert.Throws<Exception>(() => userRepository.AddUser("test1@mail.ru", "Aa1!", RoleId.User));

        // Test or getting lst of users
        Assert.Contains("test1@mail.ru", userRepository.GetUsers());

        // Test for delete user and getting guid
        var deletedGuid = userRepository.DeleteUser("test1@mail.ru");
        Assert.Equivalent(createdGuid, deletedGuid);

        // Test for delete success
        Assert.Throws<Exception>(() => userRepository.GetUserId("test1@mail.ru"));
    }

    [Fact]
    public void TestMessageRepository()
    {
        UserContext userContext = new UserContext(_userDbConnectionString);
        UserRepository userRepository = new UserRepository(userContext);

        AppDbContext messageContext = new AppDbContext(_messageDbConnectionString);
        var mapperConfiguration = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProFile>()
        );
        var mapper = mapperConfiguration.CreateMapper();
        UsersMessages.Repo.UsersMessages usersMessages = new UsersMessages.Repo.UsersMessages(mapper, messageContext);

        var countMessagesBefore = usersMessages.GetAllMessages().ToList().Count;
        userRepository.AddUser("test1@mail.ru", "Aa1!", RoleId.User);
        var user1Guid = userRepository.GetUserId("test1@mail.ru");
        userRepository.AddUser("test2@mail.ru", "Aa1!", RoleId.User);
        var user2Guid = userRepository.GetUserId("test2@mail.ru");

        // Test for adding message
        usersMessages.AddMessage(new MessageDto {ToId = user2Guid, FromId = user1Guid, Text = "Test1"});
        var messageEqual = usersMessages.GetAllUserMessages(user2Guid)
            .Any(x =>
                x.ToId == user2Guid
                && x.FromId == user1Guid
                && x.Text.Equals("Test1")
            );
        Assert.True(messageEqual);

        // Test for receiving only new message
        Assert.True(usersMessages.GetUnreadUserMessages(user2Guid).ToList().Count == 1);
        Assert.True(usersMessages.GetUnreadUserMessages(user2Guid).ToList().Count == 0);

        // Test for message count
        var countMessagesAfter = usersMessages.GetAllMessages().ToList().Count;
        Assert.True(countMessagesBefore + 1 == countMessagesAfter);

        userRepository.DeleteUser("test1@mail.ru");
        userRepository.DeleteUser("test2@mail.ru");

        Assert.True(usersMessages.GetAllMessages().ToList().Count > 0);
    }
}