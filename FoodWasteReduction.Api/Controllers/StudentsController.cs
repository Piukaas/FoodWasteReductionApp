using Microsoft.AspNetCore.Mvc;
using FoodWasteReduction.Infrastructure.Data;
using FoodWasteReduction.Core.Entities;

namespace FoodWasteReduction.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            return _context.Students.ToList();
        }
    }
}