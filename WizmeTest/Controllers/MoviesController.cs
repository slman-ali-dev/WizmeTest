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
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(DatabaseContext context, ILogger<MoviesController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/Movies
        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetMovies([FromQuery] PagingParameters pagingParameters)
        {
            var moviesCollection = _context.Movies
                            .Include(a => a.Actors)
                                .ThenInclude(ms => ms.Actor);

            PagedList<Movie> movies = new PagedList<Movie>(moviesCollection, pagingParameters.PageNumber, pagingParameters.PageSize);


            var paginationMetadata = new
            {
                totalCount = movies.TotalCount,
                pageSize = movies.PageSize,
                currentPage = movies.CurrentPage,
                totalPages = movies.TotalPages
            };
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return movies.Items;
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(long id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                _logger.LogInformation($"GetMovie movie:{id} wasn't found");
                return NotFound();
            }

            return movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(long id, Movie movie)
        {
            if (id != movie.Id)
            {
                _logger.LogWarning($"PutMovie movie id:{id} doesn't match the body.id:{movie.Id}");
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    _logger.LogInformation($"PutMovie movie:{id} wasn't found");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"PutMovie Unknown error happend while updating movie:{id} with data:{movie} ");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(long id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                _logger.LogInformation($"DeleteMovie movie:{id} wasn't found");
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return movie;
        }

        private bool MovieExists(long id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
