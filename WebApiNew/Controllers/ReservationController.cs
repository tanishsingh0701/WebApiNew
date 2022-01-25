using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiNew.Data;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly CinemaDbContext _dbContext;

        public ReservationController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Reservation reservationObj) 
        {
            reservationObj.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservationObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetReservations() 
        {
            var reservations = from reservation in _dbContext.Reservations
                               join customer in _dbContext.Users on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               select new
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = customer.Name,
                                   MovieName = movie.Name
                               };

            return Ok(reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservationDetail(int id)
        {
            var reservations = from reservation in _dbContext.Reservations
                               join customer in _dbContext.Users on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where (reservation.Id == id)
                               select new
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = customer.Name,
                                   MovieName = movie.Name,
                                   Email= customer.Email,
                                   Qty=reservation.Qty,
                                   Price=reservation.Price,
                                   Phone=reservation.Phone,
                                   playingDate=movie.PlayingDate,
                                   playingTime=movie.PlayingTime

                               };

            return Ok(reservations);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var reservation = _dbContext.Reservations.Find(id);

            if (reservation == null)
            {
                return NotFound("No record found with this id");
            }
            else
            {
                _dbContext.Reservations.Remove(reservation);
                _dbContext.SaveChanges();
                return Ok("Record deleted successfully");

            }

        }
    }
}
