using UsersMessages.Dto;

namespace UsersMessages.Services;

public interface IUserRequest
{
    public Task<ResponseMessage?> GetToken(LoginModelDto loginModelDto);

    public Task<ResponseMessage> GetUserId(string email, string token);

    public HttpClient CreateHttpClientWithJwt(string accessToken = "");
}