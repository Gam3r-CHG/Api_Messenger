using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersRegistration.Db;
using UsersRegistration.Models;
using UsersRegistration.Repo;
using UsersRegistration.Services;

namespace UsersRegistration.Controllers;

[ApiController]
[Route("[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly Validator _validator;
    private readonly AuthorizeTools _authorizeTools;

    public RegistrationController(Validator validator, IUserRepository userRepository, AuthorizeTools authorizeTools)
    {
        _userRepository = userRepository;
        _validator = validator;
        _authorizeTools = authorizeTools;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public ActionResult<LoginResult> Login([FromBody] LoginModel loginModel)
    {
        try
        {
            var roleId = _userRepository.CheckUser(loginModel.Email, loginModel.Password);

            var user = new UserModel
            {
                Email = loginModel.Email,
                Password = loginModel.Password, Role = _authorizeTools.RoleIdToRole(roleId)
            };

            var token = _authorizeTools.GenerateToken(user);

            Console.WriteLine(token);

            return Ok(new LoginResult {Token = token.ToString()});
        }
        catch (Exception e)
        {
            return StatusCode(500, new LoginResult {Error = e.Message});
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("AddAdmin")]
    public ActionResult AddAdmin([FromBody] LoginModel loginModel)
    {
        var validatorResult = _validator.ValidateLoginModel(loginModel);

        if (!validatorResult.Valid)
        {
            return BadRequest(string.Join("\n", validatorResult.Errors));
        }

        try
        {
            _userRepository.AddUser(loginModel.Email, loginModel.Password, RoleId.Admin);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("AddUser")]
    public ActionResult AddUser([FromBody] LoginModel loginModel)
    {
        var validatorResult = _validator.ValidateLoginModel(loginModel);

        if (!validatorResult.Valid)
        {
            return BadRequest(string.Join("\n", validatorResult.Errors));
        }

        try
        {
            _userRepository.AddUser(loginModel.Email, loginModel.Password, RoleId.User);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok();
    }

    [HttpGet]
    [Route("GetUsers")]
    [Authorize(Roles = "Administrator")]
    public ActionResult GetUsers()
    {
        var users = _userRepository.GetUsers();

        return Ok(users);
    }

    [HttpGet]
    [Route("GetCurrentUserId")]
    [Authorize(Roles = "Administrator, User")]
    public ActionResult<int> GetCurrentUserId()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var userClaims = identity.Claims;

            var id = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (id is not null)
            {
                return Ok(id);
            }

            return StatusCode(500, "Unknown user");
        }

        return StatusCode(500);
    }

    [HttpGet]
    [Route("GetUserId")]
    [Authorize(Roles = "Administrator, User")]
    public ActionResult<Guid> GetUserId(string email)
    {
        try
        {
            var userId = _userRepository.GetUserId(email);
            return Ok(userId);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Administrator")]
    [Route("DeleteUser")]
    public ActionResult DeleteUser(string email)
    {
        try
        {
            return Ok(_userRepository.DeleteUser(email));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}