#nullable disable
using APP.Users.Features.Users;
using CORE.APP.Features;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

//Generated from Custom Template.
namespace API.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMediator _mediator;

        public UsersController(ILogger<UsersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new UserQueryRequest());
                var list = await response.ToListAsync();
                if (list.Any())
                    return Ok(list);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersGet Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during UsersGet."));
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new UserQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);
                if (item is not null)
                    return Ok(item);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersGetById Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during UsersGetById."));
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> Post(UserCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        //return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
                        return Ok(response);
                    }
                    ModelState.AddModelError("UsersPost", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersPost Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during UsersPost."));
            }
        }

        // PUT: api/Users
        [HttpPut]
        public async Task<IActionResult> Put(UserUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        //return NoContent();
                        return Ok(response);
                    }
                    ModelState.AddModelError("UsersPut", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersPut Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during UsersPut."));
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new UserDeleteRequest() { Id = id });
                if (response.IsSuccessful)
                {
                    //return NoContent();
                    return Ok(response);
                }
                ModelState.AddModelError("UsersDelete", response.Message);
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersDelete Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during UsersDelete."));
            }
        }

        // Way 1:
        //[HttpPost("[action]")] // api/Users/Token
        // Way 2:
        [HttpPost, Route("/api/[action]")] // api/Token
        [AllowAnonymous]
        public async Task<IActionResult> Token(TokenRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                        return Ok(response);
                    ModelState.AddModelError("UsersToken", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("UsersToken Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during UsersToken."));
            }
        }

        [HttpPost, Route("/api/[action]"), AllowAnonymous] // api/RefreshToken
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(request);
                if (response.IsSuccessful)
                    return Ok(response);
                ModelState.AddModelError("UsersRefreshToken", response.Message);
            }
            return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
        }

        [HttpGet("[action]")] // GET: /api/users/authorize
        [AllowAnonymous]
        public IActionResult Authorize()
        {
            // Check if the request's identity (User) is authenticated
            var isAuthenticated = User.Identity.IsAuthenticated;

            if (isAuthenticated)
            {
                // Extract username from identity
                var userName = User.Identity.Name;

                // Check if user has the "Admin" role
                var isAdmin = User.IsInRole("Admin");

                // Read custom claims from JWT token
                var role = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var id = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;

                // Construct a friendly message to return to the caller
                var message = $"User authenticated. " +
                              $"User Name: {userName}, " +
                              $"Is Admin?: {(isAdmin ? "Yes" : "No")}, " +
                              $"Role: {role}, " +
                              $"Id: {id}";

                return Ok(new CommandResponse(true, message));
            }

            // Token was not valid or missing — user is unauthenticated
            return BadRequest(new CommandResponse(false, "User not authenticated!"));
        }
    }
}
