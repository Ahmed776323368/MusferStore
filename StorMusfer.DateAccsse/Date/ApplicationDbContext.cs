using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreMusfer.Models;

namespace StoreMusfer.DateAccsse.Date
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
       public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderHeader>().Property(or => or.PaymentDueDate).HasColumnType("date");
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Scifi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Hoistry", DisplayOrder = 3 }
                );


            modelBuilder.Entity<Company>().HasData(
               new Company { Id = 1,
                   Name = "Action",
                   StreetAdress="nbbb",
                   City="ffff",
                   State="il",
                   PostalCode="hhh",
                   PhoneNumber="776323368" },
               new Company
               {
                   Id = 2,
                   Name = "hahab",
                   StreetAdress = "nbbb",
                   City = "ffff",
                   State = "il",
                   PostalCode = "hhh",
                   PhoneNumber = "776323368"
               },
               new Company
               {
                   Id = 3,
                   Name = "ahmed",
                   StreetAdress = "nbbb",
                   City = "ffff",
                   State = "il",
                   PostalCode = "hhh",
                   PhoneNumber = "776323368"
               },
                new Company
                {
                    Id = 4,
                    Name = "Actnmmm",
                    StreetAdress = "nbbb",
                    City = "ffff",
                    State = "il",
                    PostalCode = "hhh",
                    PhoneNumber = "776323368"
                }
               );

            modelBuilder.Entity<Product>().HasData(
                new Product {Id=1,
                    Title="ASp.net" ,
                    Description="good",
                    ISBN="got1111111",
                    Author="Ahmed",
                    ListPrice=30,
                    Price=27,
                    Price100=20,
                    Price50=25,
                    CategoryId = 1,
                    ImageUrl = "d"

                },
                new Product
                {
                      Id = 2,
                      Title = "larvel",
                      Description = "good",
                      ISBN = "HH1111111",
                      Author = "ali",
                      ListPrice = 30,
                      Price = 27,
                      Price100 = 20,
                      Price50 = 25,
                      CategoryId=1,
                      ImageUrl="d"
                 },
                  new Product
                  {
                      Id = 3,
                      Title = "python",
                      Description = "good",
                      ISBN = "ttt1111111",
                      Author = "Ahmed",
                      ListPrice = 30,
                      Price = 27,
                      Price100 = 20,
                      Price50 = 25,
                      CategoryId = 2,
                      ImageUrl = "d"
                  },
                    new Product
                    {
                        Id = 4,
                        Title = "C#",
                        Description = "good",
                        ISBN = "got1111111",
                        Author = "Mohmmed",
                        ListPrice = 30,
                        Price = 27,
                        Price100 = 20,
                        Price50 = 25,
                        CategoryId = 3,
                        ImageUrl = "d"
                    }


                );
        }
       
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCard> ShoppingCards { get; set; }
        public DbSet<OrderHeader> orderHeaders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }




    }
}
