using StoreMusfer.DateAccsse.Date;
using StoreMusfer.DateAccsse.Repository.IRepository;
using StoreMusfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.DateAccsse.Repository
{
    public class ProductRepoistory : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void update(Product obj)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == obj.Id);
            if (product != null)
            {
                product.Title = obj.Title;
                product.Description = obj.Description;
                product.ISBN = obj.ISBN;
                product.Price = obj.Price;
                product.ListPrice = obj.ListPrice;
                product.Price100 = obj.Price100;
                product.Price50 = obj.Price50;
                if (obj.ImageUrl != null)
                {
                    product.ImageUrl = obj.ImageUrl;
                }
            }
            
        }
    }
}
