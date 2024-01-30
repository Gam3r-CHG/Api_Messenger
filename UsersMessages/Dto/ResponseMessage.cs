using System.Net;

namespace UsersMessages.Dto;

public class ResponseMessage
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
 }