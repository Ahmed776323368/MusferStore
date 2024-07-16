using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMusfer.DateAccsse.Repository.IRepository
{
    public interface IUnitOfWork
    {
         ICategoryRepository category { get; }
        IProductRepository product { get;  }
        ICompanyRepository company { get; }
        IShoppingCardRepository shoppingCard { get; }
        IApplicationUserRepository ApplicationUser{ get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail{ get; }
        IProductImageRepository productImage { get; }

        void Save();

    }
}
