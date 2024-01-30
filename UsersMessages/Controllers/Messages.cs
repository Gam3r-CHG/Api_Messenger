using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UsersMessages.Dto;
using UsersMessages.Repo;
using UsersMessages.Services;

namespace UsersMessages.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IUserRequest _usersRequest;
    private readonly IUsersMessages _usersMessages;
    private static string _token = "";
    private static Guid _currentUserGuid;

    public MessagesController(IUsersMessages usersMessages, IUserRequest userRequest)
    {
        _usersMessages = usersMessages;
        _usersRequest = userRequest;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login(LoginModelDto loginModelDto)
    {
        var responseMessage = await _usersRequest.GetToken(loginModelDto);

        if (responseMessage is null)
        {
            return StatusCode(500, "Server not response");
        }

        if (responseMessage.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int) responseMessage.StatusCode, responseMessage.Error);
        }

        if (!string.IsNullOrWhiteSpace(responseMessage.Message))
        {
            _token = responseMessage.Message;
            try
            {
                SetGuid();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        return StatusCode(500);
    }

    [HttpGet]
    [Route("CurrentUserId")]
    public async Task<ActionResult<Guid>> CurrentUserId()
    {
        if (string.IsNullOrWhiteSpace(_token))
        {
            return Unauthorized("Token is empty");
        }

        var securityToken = new JwtSecurityToken(_token);
        var userIdFromToken = securityToken.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdFromToken is null)
        {
            return StatusCode(500, "Token is broken");
        }

        return Ok(Guid.Parse(userIdFromToken));
    }

    [HttpGet]
    [Route("GetUserId")]
    public async Task<ActionResult<Guid>> GetUserId(string email)
    {
        var responseMessage = await _usersRequest.GetUserId(email, _token);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            return Ok(responseMessage.Message);
        }

        return StatusCode((int) responseMessage.StatusCode, responseMessage.Error);
    }

    [HttpGet]
    [Route("GetAllUserMessages")]
    public ActionResult<IEnumerable<MessageDto>> GetAllUserMessages()
    {
        try
        {
            var messages = _usersMessages.GetAllUserMessages(_currentUserGuid);
            return Ok(messages);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    [Route("ReceiveMessages")]
    public ActionResult<IEnumerable<MessageDto>> GetAllUnreadUserMessages()
    {
        try
        {
            var messages = _usersMessages.GetUnreadUserMessages(_currentUserGuid);
            return Ok(messages);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    [Route("SendMessage")]
    public async Task<ActionResult<int>> SendMessage(string emailTo, string text)
    {
        var responseMessage = await _usersRequest.GetUserId(emailTo, _token);

        if (responseMessage.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int) responseMessage.StatusCode, responseMessage.Error);
        }

        var message = new MessageDto
        {
            Text = text,
            FromId = _currentUserGuid,
            ToId = Guid.Parse(responseMessage.Message)
        };

        try
        {
            var messageId = _usersMessages.AddMessage(message);
            return Ok(messageId);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    private static void SetGuid()
    {
        var securityToken = new JwtSecurityToken(_token);
        var userIdFromToken = securityToken.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdFromToken is null)
        {
            throw new Exception("Token is broken");
        }

        _currentUserGuid = Guid.Parse(userIdFromToken);
    }
}