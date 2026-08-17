using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly UserDbContext _context;

        public ClassesController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/Classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GymClass>>> GetClasses()
        {
            return await _context.GymClasses.ToListAsync();
        }

        // GET: api/Classes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<GymClass>> GetClass(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);

            if (gymClass == null)
            {
                return NotFound(new { message = "Class not found." });
            }

            return gymClass;
        }

        // POST: api/Classes
        [HttpPost]
        public async Task<ActionResult<GymClass>> CreateClass(GymClass gymClass)
        {
            _context.GymClasses.Add(gymClass);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetClass),
                new { id = gymClass.Id },
                gymClass
            );
        }
    }
}