using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOFTITO_Project.Data;
using SOFTITO_Project.Models;

namespace SOFTITO_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public FoodsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Foods
        [HttpGet]
        [Authorize(Roles = "Admin,CompanyAdmin,RestaurantAdmin,RestaurantBranchAdmin,MenuAdmin")]
        public async Task<ActionResult<IEnumerable<Food>>> GetFood()
        {
          if (_context.Food == null)
          {
              return NotFound();
          }
            return await _context.Food.ToListAsync();
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,CompanyAdmin,RestaurantAdmin,RestaurantBranchAdmin,MenuAdmin")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
          if (_context.Food == null)
          {
              return NotFound();
          }
            var food = await _context.Food.FindAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }
        

        // PUT: api/Foods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantBranchAdmin,MenuAdmin")]
        public async Task<IActionResult> PutFood(int id, Food food)
        {
            var currentUser = HttpContext.User;
            var restaurantBranchIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantBranchId");
            if (restaurantBranchIdClaim == null || restaurantBranchIdClaim.Value != food.BranchId.ToString())
            {
                return StatusCode(403); // Yetkisiz erişim hatası
            }
            if (id != food.Id)
            {
                return BadRequest();
            }
            _context.Entry(food).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }           
            return NoContent();
        }

        // POST: api/Foods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "RestaurantBranchAdmin,MenuAdmin")]
        public IActionResult PostFood(Food food)
        {
            var currentUser = HttpContext.User;
            var restaurantBranchIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantBranchId");
            if (restaurantBranchIdClaim == null || restaurantBranchIdClaim.Value != food.BranchId.ToString())
            {
                return StatusCode(403); // Yetkisiz erişim hatası
            }
            if (_context.Food == null)
            {
              return Problem("Entity set 'ApplicationContext.Food'  is null.");
            }
          
                _context.Food.Add(food);
                _context.SaveChangesAsync().Wait();

            return CreatedAtAction("GetFood", new { id = food.Id }, food);
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,MenuAdmin,RestaurantBranchAdmin")]
        public async Task<IActionResult> DeleteFood(int id) //Başka bir şubenin de yemeğini siliyor 
        {
            var food = await _context.Food.FindAsync(id);

            var currentUser = HttpContext.User;
            var restaurantBranchIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantBranchId");
            if (restaurantBranchIdClaim == null || restaurantBranchIdClaim.Value != food.BranchId.ToString())
            {
                return StatusCode(403); // Yetkisiz erişim hatası
            }
            if (_context.Food == null)
            {
                return NotFound();
            }
            if (food == null)
            {
                return NotFound();
            }
            int branchid = Convert.ToInt32(restaurantBranchIdClaim.Value);
            
            if(food.BranchId == branchid) //Böylece menü admin veya bir başkası başka menüdeki foodları silemeyecek
            {
                food.StateId = 0;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            return NotFound();
        }

        private bool FoodExists(int id)
        {
            return (_context.Food?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
