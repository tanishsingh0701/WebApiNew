using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApiNew.Data;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("[action]")]
        [Authorize]
        public IActionResult AllMovies(string sort,int? pageNumber,int? pageSize)
        {
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize=pageSize ?? 5;

            var movies = from movie in _dbContext.Movies
                         select new 
                         {
                             Id=movie.Id,
                             Name=movie.Name,
                             Duration=movie.Duration,
                             Language=movie.Language,
                             Rating=movie.Rating,
                             Genre=movie    .Genre,
                             ImageUrl=movie.ImageUrl
                         };
            //return _dbContext.Movies;
            // here ok method will return movie and status both

            switch (sort) 
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize)  .OrderBy(m => m.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber-1)*currentPageSize).Take(currentPageSize));
            }
           
        }

        // GET api/<MoviesController>/5
        [HttpGet("[action]/{id}")]
        [Authorize]
        public IActionResult MovieDetail(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            
            if(movie == null) 
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName) 
        {
            var movies = from movie in _dbContext.Movies
                         where(movie.Name.StartsWith(movieName))
                             select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duration = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl
                         };

            return Ok(movies);


        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            // here new guid will be created and used for name of image
            var guid = Guid.NewGuid();

            //here combine method will string to path type
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj.Image != null)
            {
                var fileSteam = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileSteam);

            }
            movieObj.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("There is no record found with this id");
            }
            else
            {

                var guid = Guid.NewGuid();

                //here combine method will string to path type
                var filePath = Path.Combine("wwwroot", guid + ".jpg");
                if (movie.Image != null)
                {
                    var fileSteam = new FileStream(filePath, FileMode.Create);
                    movie.Image.CopyTo(fileSteam);
                    movie.ImageUrl = filePath.Remove(0, 7);

                }

                movie.Name = movieObj.Name;
                movie.Language = movieObj.Language;
                movie.Rating = movieObj.Rating;
                movie.Description = movieObj.Description;
                movie.Duration = movieObj.Duration;
                movie.Genre = movieObj.Genre;
                movie.PlayingDate = movieObj.PlayingDate;
                movie.PlayingTime = movieObj.PlayingTime;
                movie.TrailorUrl = movieObj.TrailorUrl;
                movie.TicketPrice = movieObj.TicketPrice;


                _dbContext.SaveChanges();
                return Ok("Record Updated Successfully");

            }
        }

            [HttpDelete("{id}")]
            [Authorize(Roles ="Admin")]
            public IActionResult Delete(int id)
            {
                var movie = _dbContext.Movies.Find(id);

                if (movie == null)
                {
                    return NotFound("No record found with this id");
                }
                else
                {
                    _dbContext.Movies.Remove(movie);
                    _dbContext.SaveChanges();
                    return Ok("Record deleted successfully");

                }

            }



        }
    }

