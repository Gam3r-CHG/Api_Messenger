using UsersMessages.Dto;

namespace UsersMessages.Repo;

public interface IUsersMessages
{
    public IEnumerable<MessageDto> GetAllMessages();
    public IEnumerable<MessageDto> GetAllUserMessages(Guid userId);
    public IEnumerable<MessageDto> GetUnreadUserMessages(Guid userId);
    public int AddMessage(MessageDto message);
}