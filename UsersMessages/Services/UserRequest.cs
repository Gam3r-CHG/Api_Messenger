using UsersMessages.Dto;

namespace UsersMessages.Services;

public class UserRequest: IUserRequest
{
    private readonly string _serverAddress;

    public UserRequest(IConfiguration configuration)
    {
        _serverAddress = configuration["ServerAddress"];
    }

    public async Task<ResponseMessage?> GetToken(LoginModelDto loginModelDto)
    {
        using var httpClient = new HttpClient();
        JsonContent content = JsonContent.Create(loginModelDto);
        using var response = await httpClient.PostAsync($"{_serverAddress}Registration/Login", content);
        var loginResult = await response.Content.ReadFromJsonAsync<LoginResultDto>();
        return new ResponseMessage
            {StatusCode = response.StatusCode, Message = loginResult?.Token, Error = loginResult?.Error};
    }

    public async Task<ResponseMessage> GetUserId(string email, string token)
    {
        using var httpClient = CreateHttpClientWithJwt(token);
        using var response = await httpClient.GetAsync($"{_serverAddress}Registration/GetUserId?email={email}");
        string result;
        if (response.Content.Headers.ContentType?.MediaType == "application/json")
        {
            var guid = await response.Content.ReadFromJsonAsync<Guid>();
            result = guid.ToString();
        }
        else
        {
            result = await response.Content.ReadAsStringAsync();
        }

        return new ResponseMessage {StatusCode = response.StatusCode, Message = result};
    }

    public HttpClient CreateHttpClientWithJwt(string accessToken = "")
    {
        var client = new HttpClient();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        return client;
    }
}