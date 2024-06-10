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
    public class ShoppingCardRepository : Repository<ShoppingCard>, IShoppingCardRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCardRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(ShoppingCard obj)
        {
            _db.ShoppingCards.Update(obj);
        }
    }
}
