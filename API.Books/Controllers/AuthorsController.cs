#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CORE.APP.Features;
using APP.Books.Features.Authors;
using Microsoft.AspNetCore.Authorization;

namespace API.Books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private readonly IMediator _mediator;

        public AuthorsController(ILogger<AuthorsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new AuthorQueryRequest());
                var list = await response.ToListAsync();

                if (list.Any())
                {
                    return Ok(list);

                }

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("AuthorsGet Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during AuthorsGet.")); 
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new AuthorQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);

                if (item is not null)
                {
                    return Ok(item);

                }

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("AuthorsGetById Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during AuthorsGetById.")); 
            }
        }

		// POST: api/Authors
        [HttpPost]
        public async Task<IActionResult> Post(AuthorCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
                    }
                    ModelState.AddModelError("AuthorsPost", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("AuthorsPost Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during AuthorsPost.")); 
            }
        }

        // PUT: api/Authors
        [HttpPut]
        public async Task<IActionResult> Put(AuthorUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        return Ok(response);
                    }
                    ModelState.AddModelError("AuthorsPut", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("AuthorsPut Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during AuthorsPut.")); 
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new AuthorDeleteRequest() { Id = id });
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                ModelState.AddModelError("AuthorsDelete", response.Message);
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("AuthorsDelete Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during AuthorsDelete.")); 
            }
        }
	}
}
