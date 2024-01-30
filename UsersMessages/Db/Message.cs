namespace UsersMessages.Db;

public partial class Message
{
    public int Id { get; set; }

    public Guid FromId { get; set; }

    public Guid ToId { get; set; }

    public string Text { get; set; }

    public bool Received { get; set; }
}
