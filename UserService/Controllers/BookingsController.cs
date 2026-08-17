using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public BookingsController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        // GET: api/Bookings/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return booking;
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = booking.Id },
                booking
            );
        }

        // PUT: api/Bookings/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(
            int id,
            Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest(new
                {
                    message = "Booking ID mismatch."
                });
            }

            var existingBooking = await _context.Bookings.FindAsync(id);

            if (existingBooking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            existingBooking.UserId = booking.UserId;
            existingBooking.GymClassId = booking.GymClassId;
            existingBooking.BookingDate = booking.BookingDate;

            await _context.SaveChangesAsync();

            return Ok(existingBooking);
        }

        // DELETE: api/Bookings/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking deleted successfully."
            });
        }
    }
}