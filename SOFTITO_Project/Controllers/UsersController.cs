using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOFTITO_Project.Data;
using SOFTITO_Project.Models;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace SOFTITO_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationContext context, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(User user, string passWord)
        {
            var result = await _signInManager.UserManager.CreateAsync(user, passWord);
            if (result.Succeeded)
            {
                var myuser = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                return myuser.Id;
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("LogIn")]
        public bool LogIn(string userName, string passWord)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            User applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            Claim claim;

            if (applicationUser == null)
            {
                return false;
            }
            signInResult = _signInManager.PasswordSignInAsync(applicationUser, passWord, false, false).Result;
            if (signInResult.Succeeded)
            {
                 _signInManager.SignInAsync(applicationUser, isPersistent: false).Wait();
                 _userManager.AddClaimAsync(applicationUser, new Claim("CompanyId", applicationUser.CompanyId.ToString()));
            }
            return signInResult.Succeeded;
        }
        

        [Authorize(Roles = "Admin")]
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }
        [Authorize(Roles = "Admin")]
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationContext.Users'  is null.");
            }
            user.RegisterDate = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("id")]
        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Bu id ye sahip bir kullanıcı bulunamadı");
            }
            user.StateId = 0;
            await _context.SaveChangesAsync();
            return Ok("Kullanıcı başarıyla silindi");
        }

        [HttpPost("AssignRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(string userId,string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if(!roleExist)
            {
                return NotFound("Sistemde Admin rolü oluşturulmamış");
            }
            User applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole identityRole = _roleManager.FindByIdAsync(roleName).Result;

            var result = await _signInManager.UserManager.AddToRoleAsync(applicationUser, roleName);

           // var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                return Ok("Rol kullanıcıya başarıyla atandı");
            }
            else
            {
                return BadRequest(result.Errors);
            }


        }

        
        [HttpPost("AssignClaim")]
        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> AssignClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var claim = new Claim("CompanyId", user.CompanyId.ToString());

                var result = await _userManager.AddClaimAsync(user, claim);

                if (result.Succeeded)
                {
                    return Ok(); // Talep başarıyla eklendi.
                }
                else
                {
                    return BadRequest(); // Talep eklenirken bir hata oluştu.
                }
            }
            return NotFound();
        }

        [HttpPost("AssignMenuAdminRole")]
        [Authorize(Roles = "RestaurantBranchAdmin,Admin")]
        public async Task<IActionResult> AssignMenuAdminRole(string userId, string roleName = "MenuAdmin")
        {
            var currentUser = HttpContext.User;
            var restaurantBranchIdClaim = currentUser.Claims.FirstOrDefault(c => c.Type == "RestaurantBranchId");
            if (restaurantBranchIdClaim == null)
            {
                return StatusCode(403); // Yetkisiz erişim hatası
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return NotFound("Sistemde MenuAdmin rolü oluşturulmamış");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                Claim claim = new Claim("RestaurantBranchId", restaurantBranchIdClaim.Value);
                var claimResult = await _userManager.AddClaimAsync(user, claim);
                if (claimResult.Succeeded)
                {
                    return Ok("Kullanıcıya rol ve claim başarıyla atandı");
                }
                else
                {
                    return BadRequest(claimResult.Errors);
                }
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

    }
}
