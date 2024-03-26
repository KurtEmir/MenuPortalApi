using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
    public class RestaurantBranchesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        private string TurkceKarakterleriDonustur(string input)
        {
            StringBuilder sb = new StringBuilder(input);

            sb = sb.Replace("ç", "c")
                   .Replace("Ç", "C")
                   .Replace("ş", "s")
                   .Replace("Ş", "S")
                   .Replace("ı", "i")
                   .Replace("İ", "I")
                   .Replace("ğ", "g")
                   .Replace("Ğ", "G")
                   .Replace("ü", "u")
                   .Replace("Ü", "U")
                   .Replace("ö", "o")
                   .Replace("Ö", "O")
                   .Replace("â", "a")
                   .Replace("Â", "A")
                   .Replace("î", "i")
                   .Replace("Î", "I")
                   .Replace("û", "u")
                   .Replace("Û", "U")
                   .Replace("ë", "e")
                   .Replace("Ë", "E")
                   .Replace("î", "i")
                   .Replace("Î", "I");

            return sb.ToString();
        }

        public RestaurantBranchesController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: api/RestaurantBranches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantBranch>>> GetRestaurantBranch()
        {
          if (_context.RestaurantBranch == null)
          {
              return NotFound();
          }
            return await _context.RestaurantBranch.ToListAsync();
        }

        // GET: api/RestaurantBranches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantBranch>> GetRestaurantBranch(int id)
        {
          if (_context.RestaurantBranch == null)
          {
              return NotFound();
          }
            var restaurantBranch = await _context.RestaurantBranch.FindAsync(id);

            if (restaurantBranch == null)
            {
                return NotFound();
            }

            return restaurantBranch;
        }

        // PUT: api/RestaurantBranches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantBranchAdmin")]
        public async Task<IActionResult> PutRestaurantBranch(int id, RestaurantBranch restaurantBranch)
        {
            var currentUser = HttpContext.User;
            var restaurantIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantId");
            if (restaurantIdClaim == null || restaurantIdClaim.Value != restaurantBranch.RestaurantId.ToString())
            {
                return StatusCode(403); // Yetkisiz erişim hatası
            }

            if (id != restaurantBranch.Id)
            {
                return BadRequest();
            }
            if (User.HasClaim("RestaurantBranchId", id.ToString()) == true)
            {
                _context.Entry(restaurantBranch).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantBranchExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return NotFound("Bu şubeyi güncellemek için uygun izniniz bulunmamaktadır.");
            }

            return NoContent();
        }

        // POST: api/RestaurantBranches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "RestaurantAdmin")]
        public IActionResult PostRestaurantBranch(RestaurantBranch restaurantBranch)
        {
            var currentUser = HttpContext.User;
            var restaurantIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantId");
            if (restaurantIdClaim == null || restaurantIdClaim.Value != restaurantBranch.RestaurantId.ToString())
            {
                return StatusCode(403); // Yetkisiz erişim hatası
            }

            User applicationUser = new User();
            Claim claim;
            Restaurant restaurant = new Restaurant();
            _context.RestaurantBranch.Add(restaurantBranch);
            _context.SaveChangesAsync().Wait();

            int restaurantCompanyId =  _context.Restaurants
                                  .Where(r => r.Id == restaurantBranch.RestaurantId)
                                  .Select(r => r.CompanyId)
                                  .FirstOrDefault();
            try
            {
                string UserName= restaurantBranch.Name + restaurantBranch.Id.ToString();
                string newUserName= TurkceKarakterleriDonustur(UserName);
                string bosluksuzString = newUserName.Replace(" ", "");

                applicationUser.UserName = bosluksuzString;
                applicationUser.Name = restaurantBranch.Name;
                applicationUser.Email = "";
                applicationUser.PhoneNumber = "";
                applicationUser.RegisterDate = restaurantBranch.RegisterDate;
                applicationUser.StateId = restaurantBranch.StateId;
                applicationUser.CompanyId = restaurantCompanyId; //Burda güzel bi bağlantı var 
                applicationUser.PhoneNumber = "";
                _userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                claim = new Claim("RestaurantBranchId", restaurantBranch.Id.ToString());
                _userManager.AddClaimAsync(applicationUser, claim).Wait();
                _userManager.AddToRoleAsync(applicationUser, "RestaurantBranchAdmin").Wait(); // Kullanıcı oluşturma işlemleri
            }
            catch (Exception ex)
            {
                // Hata durumunda yapılacak işlemler
                Console.WriteLine(ex.Message); // Hata mesajını yazdırabilirsiniz
            }

           
            return Ok(restaurantBranch.Id);
        }

        // DELETE: api/RestaurantBranches/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RestaurantAdmin")]
        public async Task<IActionResult> DeleteRestaurantBranch(int id)
        {
            var restaurantBranch = await _context.RestaurantBranch.FindAsync(id);

            var currentUser = HttpContext.User;
            var restaurantBranchIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantId");
            if (restaurantBranchIdClaim.Value != restaurantBranch.RestaurantId.ToString())
            {
                return Unauthorized();
            }
            if (_context.RestaurantBranch == null)
            {
                return NotFound();
            }
            if (restaurantBranch == null)
            {
                return NotFound();
            }

            return NoContent();
        }
       
        private bool RestaurantBranchExists(int id)
        {
            return (_context.RestaurantBranch?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
