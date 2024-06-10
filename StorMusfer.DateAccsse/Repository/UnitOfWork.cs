using StoreMusfer.DateAccsse.Date;
using StoreMusfer.DateAccsse.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.DateAccsse.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            category = new CategoryRepository(_db);
            product = new ProductRepoistory(_db);
            company = new CompanyRepository(_db);
            shoppingCard = new ShoppingCardRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail= new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
        }
        public ICategoryRepository category { get; private set; }
        public IProductRepository product { get; private set; }
        public ICompanyRepository company { get; private set; }
        public IShoppingCardRepository shoppingCard { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
