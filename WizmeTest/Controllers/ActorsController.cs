using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WizmeTest.DbContexts;
using WizmeTest.Helpers;
using WizmeTest.Models;
using WizmeTest.ResourceParameters;

namespace WizmeTest.Controllers
{
    [Route("api/actors")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ILogger<ActorsController> _logger;

        public ActorsController(DatabaseContext context, ILogger<ActorsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/Actors
        [HttpGet]
        public ActionResult<IEnumerable<Actor>> GetActors(
            [FromQuery] PagingParameters pagingParameters)
        {
            var actorsCollection = _context.Actors
                            .Include(a => a.Movies)
                                .ThenInclude(ms => ms.Movie);

            PagedList<Actor> actors = new PagedList<Actor>(actorsCollection, pagingParameters.PageNumber, pagingParameters.PageSize);


            var paginationMetadata = new
            {
                totalCount = actors.TotalCount,
                pageSize = actors.PageSize,
                currentPage = actors.CurrentPage,
                totalPages = actors.TotalPages
            };
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return actors.Items;
        }

        // GET: api/Actors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(long id)
        {
            //var actor = await _context.Actors.FindAsync(id);
            var actor = await _context.Actors.SingleAsync(a => a.Id == id);

            _context.Entry(actor)
                .Collection(a => a.Movies)
                .Query()
                .Include(a => a.Movie)
                .Load();

            if (actor == null)
            {
                _logger.LogInformation($"GetActor actor:{id} wasn't found");
                return NotFound();
            }

            return actor;
        }

        // PUT: api/Actors/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActor(long id, Actor actor)
        {
            if (id != actor.Id)
            {
                _logger.LogWarning($"PutActor actor id:{id} doesn't match the body.id:{actor.Id}");
                return BadRequest();
            }

            _context.Entry(actor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
                {
                    _logger.LogInformation($"PutActor actor:{id} wasn't found");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"PutActor Unknown error happend while updating actor:{id} with data:{actor} ");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Actors
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor(Actor actor)
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        // DELETE: api/Actors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Actor>> DeleteActor(long id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                _logger.LogInformation($"DeleteActor actor:{id} wasn't found");
                return NotFound();
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return actor;
        }

        private bool ActorExists(long id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}
