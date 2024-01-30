namespace UsersMessages.Dto;

public class MessageDto
{
    public Guid FromId { get; set; }

    public Guid ToId { get; set; }

    public string Text { get; set; }
}