using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApiNew.Data;
using WebApiNew.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController0 : ControllerBase
    {
        private readonly CinemaDbContext _dbContext;

        public MoviesController0(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET: api/<MoviesController>

        //[HttpGet]
        //public IEnumerable<Movie> Get()
        //{
        //    //return _dbContext.Movies;
        //    // here ok method will return movie and status both
        //    return Ok()
        //}


        [HttpGet]
        public IActionResult Get()
        {
            //return _dbContext.Movies;
            // here ok method will return movie and status both
            return Ok(_dbContext.Movies);
        }

        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public Movie Get(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            return movie;
        }

        // Attrubute routing-like if we want to call different Get method with same parameter bt diff name

        [HttpGet("[action]/{id}")]
        // here the ur will become api/<MoviesController>/Test/5

        public int Test(int id) 
        {
            return id;
        }

        // POST api/<MoviesController>
        //[HttpPost]
        //public IActionResult Post([FromBody] Movie movieObj)
        //{
        //    _dbContext.Movies.Add(movieObj);
        //    _dbContext.SaveChanges();
        //    return StatusCode(StatusCodes.Status201Created);
        //}

        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            // here new guid will be created and used for name of image
            var guid = Guid.NewGuid();

            //here combine method will string to path type
            var filePath = Path.Combine("wwwroot",guid+".jpg");
            if(movieObj.Image != null) 
            {
                var fileSteam = new FileStream(filePath,FileMode.Create);
                movieObj.Image.CopyTo(fileSteam);

            }
            movieObj.ImageUrl = filePath.Remove(0,7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();
            return Ok();
        }

        // PUT api/<MoviesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] Movie movieObj)
        //{
        //    var movie = _dbContext.Movies.Find(id);
        //    movie.Name = movieObj.Name;
        //    movie.Language = movieObj.Language;
        //    _dbContext.SaveChanges();

        //}

        //[HttpPut("{id}")]
        //public IActionResult Put(int id, [FromBody] Movie movieObj)
        //{
        //    var movie = _dbContext.Movies.Find(id);

        //    if(movie == null) 
        //    {
        //        return NotFound("There is no record found with this id");
        //    }
        //    else
        //    {
        //        movie.Name = movieObj.Name;
        //        movie.Language = movieObj.Language;
        //        movie.Rating = movieObj.Rating;
        //        _dbContext.SaveChanges();
        //        return Ok("Record Updated Successfully");

        //    }


        //}

        [HttpPut("{id}")]
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
                

                _dbContext.SaveChanges();
                return Ok("Record Updated Successfully");

            }


        }

        // DELETE api/<MoviesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //    var movie = _dbContext.Movies.Find(id);
        //    _dbContext.Movies.Remove(movie);
        //    _dbContext.SaveChanges();
        //}

        [HttpDelete("{id}")]
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
