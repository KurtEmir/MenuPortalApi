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
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly SignInManager<User> _signInManager;
        public CompaniesController(ApplicationContext context, SignInManager<User> signInManager )
        {
            _context = context;
            _signInManager = signInManager; 
        }

        // GET: api/Companies
        [HttpGet]
        [Authorize(Roles = "CompanyAdmin,Admin")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        [Authorize(Roles = "CompanyAdmin,Admin")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdmin,Admin")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }
            if ((User.HasClaim("CompanyId", id.ToString()) == false) && (User.IsInRole("Admin") == false) && (User.IsInRole("CompanyAdmin") == false))
            {
                return Unauthorized("Bu şirketi güncellemek için uygun izniniz bulunmamaktadır.");

            }
            
                _context.Entry(company).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(id))
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


        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public int PostCompany(Company company)
        {
            User applicationUser = new User();
            Claim claim;
            _context.Companies.Add(company);
            _context.SaveChanges();
            applicationUser.CompanyId = company.Id;
            applicationUser.Email = "";
            applicationUser.Name = company.Name;
            applicationUser.PhoneNumber = company.Phone;
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = company.Name + company.Id.ToString();
            _signInManager.UserManager.CreateAsync(applicationUser, "Admin123!").Wait();
            claim = new Claim("CompanyId", company.Id.ToString());
            _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();
            _signInManager.UserManager.AddToRoleAsync(applicationUser, "CompanyAdmin").Wait();
            return company.Id;
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin") == true)
            {
                company.StateId = 0;
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound("Bu şirketi silmek için uygun izniniz bulunmamaktadır.");
            }


            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}