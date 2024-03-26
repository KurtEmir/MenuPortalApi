using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SOFTITO_Project.Data;
using SOFTITO_Project.Models;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace SOFTITO_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            State state;
            IdentityRole identityRole;
            User applicationUser;
            Company? company = null;
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<ApplicationContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
            builder.Services.AddIdentity<User, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            /* builder.Services.AddAuthorization(options =>
                     options.AddPolicy("CompAdmin",
                     policy => policy.RequireClaim("CompanyId")));*/

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            UserManager<User>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<User>>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            {
                ApplicationContext? context = app.Services.CreateScope().ServiceProvider.GetService<ApplicationContext>();
                if(context!= null)
                {
                    context.Database.Migrate();
                    if(context.States.Count() == 0)
                    {
                        state = new State();
                        state.Id = 0;
                        state.Name = "Deleted";
                        context.States.Add(state);
                        state = new State();
                        state.Id = 1;
                        state.Name = "Active";
                        context.States.Add(state);
                        state = new State();
                        state.Id = 2;
                        state.Name = "Passive";
                        context.States.Add(state);
                    }
                    if(context.Companies.Count() == 0)
                    {
                        company = new Company();
                        company.Name = "Company";
                        company.PostalCode = "12345";
                        company.Address = "adres";
                        company.Phone = "1112223344";
                        company.EMail = "abc@def.com";
                        company.RegisterDate = DateTime.Today;
                        company.TaxNumber = "11111111111";
                        company.WebAddress = "company@hotmail.com";
                        company.StateId = 1;
                        context.Companies.Add(company);
                    }
                   
                    context.SaveChanges();
                    RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
                    if (roleManager != null)
                    {
                        if (roleManager.Roles.Count() == 0)
                        {
                            identityRole = new IdentityRole("Admin");
                            roleManager.CreateAsync(identityRole).Wait();
                            identityRole = new IdentityRole("CompanyAdmin");
                            roleManager.CreateAsync(identityRole).Wait();
                            identityRole = new IdentityRole("RestaurantAdmin");
                            roleManager.CreateAsync(identityRole).Wait();
                            identityRole = new IdentityRole("RestaurantBranchAdmin");
                            roleManager.CreateAsync(identityRole).Wait();
                            identityRole = new IdentityRole("MenuAdmin");
                            roleManager.CreateAsync(identityRole).Wait();
                        }
                    }
                    if (userManager != null)
                    {
                        if (userManager.Users.Count() == 0)
                        {
                            if (company != null)
                            {
                                applicationUser = new User();
                                applicationUser.UserName = "akadekart";
                                applicationUser.CompanyId = company.Id;
                                applicationUser.Name = "Emir Kurt";
                                applicationUser.Email = "emirkurt_2001@outlook.com";
                                applicationUser.PhoneNumber = "11122233441";
                                applicationUser.RegisterDate = DateTime.Today;
                                applicationUser.StateId = 1;
                                userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                                userManager.AddToRoleAsync(applicationUser, "Admin").Wait();
                            }
                        }
                    }

                }
            }

            app.Run();
        }
    }
}
