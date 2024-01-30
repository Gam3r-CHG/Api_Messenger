using AutoMapper;
using UsersMessages.Db;
using UsersMessages.Dto;

namespace UsersMessages.Repo;

public class UsersMessages : IUsersMessages
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public UsersMessages(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public IEnumerable<MessageDto> GetAllMessages()
    {
        try
        {
            var messages = _context.Messages.Select(_mapper.Map<MessageDto>).ToList();
            return messages;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public IEnumerable<MessageDto> GetAllUserMessages(Guid userId)
    {
        try
        {
            var messages = _context.Messages
                .Where(x => x.ToId == userId)
                .Select(_mapper.Map<MessageDto>)
                .ToList();
            return messages;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public IEnumerable<MessageDto> GetUnreadUserMessages(Guid userId)
    {
        try
        {
            var messagesDb = _context.Messages
                .Where(x => x.ToId == userId && x.Received == false).ToList();
            messagesDb.ForEach(x => x.Received = true);
            _context.SaveChanges();

            var messagesDto = messagesDb.Select(_mapper.Map<MessageDto>).ToList();

            return messagesDto;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public int AddMessage(MessageDto message)
    {
        var messageDb = _mapper.Map<Message>(message);
        _context.Messages.Add(messageDb);
        _context.SaveChanges();
        return messageDb.Id;
    }
}