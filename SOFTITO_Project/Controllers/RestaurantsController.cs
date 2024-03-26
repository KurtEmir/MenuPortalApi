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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SOFTITO_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public RestaurantsController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Restaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            return await _context.Restaurants.ToListAsync();
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
          if (_context.Restaurants == null)
          {
              return NotFound();
          }
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles ="RestaurantAdmin")]
        public async Task<IActionResult> PutRestaurant(int id, Restaurant restaurant)
        {
            var currentUser = HttpContext.User;
            var companyIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null || companyIdClaim.Value != restaurant.CompanyId.ToString())
            {
                return StatusCode(403);
            }

            if (id != restaurant.Id)
            {
                return BadRequest();
            }
                _context.Entry(restaurant).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            return Content("Updated");
        }

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdmin")]
        public IActionResult PostRestaurant(Restaurant restaurant)
        {          
            var currentUser = HttpContext.User;
            var companyIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null || companyIdClaim.Value != restaurant.CompanyId.ToString())
            {
                 return StatusCode(403); 
            }

            User applicationUser = new User();
            Claim claim;
            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();

            applicationUser.UserName = restaurant.Name + restaurant.Id.ToString();
            applicationUser.Name = restaurant.Name;
            applicationUser.Email = "";
            applicationUser.PhoneNumber = "";
            applicationUser.RegisterDate = restaurant.RegisterDate;
            applicationUser.StateId = restaurant.StateId;
            applicationUser.CompanyId = restaurant.CompanyId;
            applicationUser.PhoneNumber = "";
            _userManager.CreateAsync(applicationUser, "Admin123!").Wait(); //id si 3
            claim = new Claim("RestaurantId", restaurant.Id.ToString()); //restaurant idsi 2
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            _userManager.AddToRoleAsync(applicationUser, "RestaurantAdmin").Wait();
            return Ok(restaurant.Id);
        }
        //// DELETE: api/Restaurants/5
        //[HttpDelete("{id}")]
        //public ActionResult DeleteRestaurant(int mid)
        //{
        //    if (_context.Restaurants == null)
        //    {
        //        return NotFound();
        //    }
        //    var restaurant = _context.Restaurants.Find(mid);

        //    _context.Restaurants.Remove(restaurant);

        //    _context.SaveChanges();
        //    return Content("Deleted");
        //}

        //DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantAdmin,CompanyAdmin")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var currentUser = HttpContext.User;
            var companyIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (companyIdClaim.Value != restaurant.CompanyId.ToString())
            {
                return Unauthorized();
            }
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            if (restaurant == null)
            {
                return NotFound();
            }
            restaurant.StateId = 0;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
