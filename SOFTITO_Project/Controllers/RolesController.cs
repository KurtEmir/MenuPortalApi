using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SOFTITO_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            // Rolü oluştur
            var role = new IdentityRole(roleName);

            // Role Manager ile roleü veritabanına ekleyin
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                // Başarılı bir şekilde rol eklenirse
                return Ok("Rol başarıyla eklendi.");
            }
            else
            {
                // Hata durumunda
                return BadRequest("Rol eklenirken bir hata oluştu.");
            }
        }

    }
}
