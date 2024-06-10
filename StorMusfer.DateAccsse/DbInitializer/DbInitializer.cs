using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreMusfer.DateAccsse.Date;
using StoreMusfer.Models;
using StoreMusfer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.DateAccsse.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {


            _userManager = userManager;
            _roleManager = roleManager;
            _db=db;
        }




        public void Initialize()
        {
            //migration if thay are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex){ 
            }
            //creat Role if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                //creat  user admin if they are not craeted
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "Admin1@gmail.com",
                    Email = "Admin1@gmail.com",
                    Name = "Ahmed Musfer",
                    PhoneNumber = "776323368",
                    StreetAdress = "sarf",
                    State = "Il",
                    PostalCode = "1234",
                    City = "sana",


                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(us => us.Email == "Admin1@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }

            return;

         

        }
    }
}
