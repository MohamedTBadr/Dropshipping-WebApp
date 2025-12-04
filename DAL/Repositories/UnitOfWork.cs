using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DropShoppingDbContext _context;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IBrandRepository> _brandRepository;
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IDropshipperRepository> _dropShipperRepository;
        public UnitOfWork(DropShoppingDbContext context, IProductRepository productRepository, ICategoryRepository categoryRepository,IBrandRepository brandRepository , IOrderRepository orderRepository)
        {
            _context = context;
            _productRepository = new Lazy<IProductRepository>(() => productRepository);
            _categoryRepository = new Lazy<ICategoryRepository>(() => categoryRepository);
            _brandRepository = new Lazy<IBrandRepository>(() => brandRepository);
            _orderRepository = new Lazy<IOrderRepository>(() => orderRepository);
        }
        public IProductRepository ProductRepository => _productRepository.Value;
        public ICategoryRepository CategoryRepository => _categoryRepository.Value;
        public IBrandRepository BrandRepository => _brandRepository.Value;

        public IOrderRepository orderRepository => _orderRepository.Value;

        public IDropshipperRepository DropshipperRepository => _dropShipperRepository.Value;


        public async Task<int> SaveChangesAsync()
        {
          return  await _context.SaveChangesAsync();
        }
    }
}
