using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Widgets.ProductRibbon.Infrastructure;
using ProductRibbonEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbon;
using ProductRibbonMappingEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbonMapping;

namespace Nop.Plugin.Widgets.ProductRibbon.Services
{
    public class ProductRibbonService : IProductRibbonService
    {
        private readonly IRepository<ProductRibbonEntity> _productRibbonRepository;
        private readonly IRepository<ProductRibbonMappingEntity> _productRibbonMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        public ProductRibbonService(
            IRepository<ProductRibbonEntity> productRibbonRepository,
            IRepository<ProductRibbonMappingEntity> productRibbonMappingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _productRibbonRepository = productRibbonRepository;
            _productRibbonMappingRepository = productRibbonMappingRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<IList<ProductRibbonEntity>> GetAllRibbonsAsync()
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ProductRibbonDefaults.ProductRibbonAllCacheKey);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                return await _productRibbonRepository.GetAllAsync(query => query.OrderBy(x => x.Id));
            });
        }

        public async Task<ProductRibbonEntity> GetRibbonByIdAsync(int ribbonId)
        {
            if (ribbonId <= 0)
                return null;

            return await _productRibbonRepository.GetByIdAsync(ribbonId);
        }

        public async Task InsertRibbonAsync(ProductRibbonEntity ribbon)
        {
            await _productRibbonRepository.InsertAsync(ribbon);
            await _staticCacheManager.RemoveByPrefixAsync(ProductRibbonDefaults.ProductRibbonPrefixCacheKey);
        }

        public async Task UpdateRibbonAsync(ProductRibbonEntity ribbon)
        {
            await _productRibbonRepository.UpdateAsync(ribbon);
            await _staticCacheManager.RemoveByPrefixAsync(ProductRibbonDefaults.ProductRibbonPrefixCacheKey);
        }

        public async Task DeleteRibbonAsync(ProductRibbonEntity ribbon)
        {
            await _productRibbonRepository.DeleteAsync(ribbon);
            await _staticCacheManager.RemoveByPrefixAsync(ProductRibbonDefaults.ProductRibbonPrefixCacheKey);
        }

        public async Task<IList<ProductRibbonMappingEntity>> GetMappingsByRibbonIdAsync(int ribbonId)
        {
            return await _productRibbonMappingRepository.GetAllAsync(query =>
                query.Where(x => x.ProductRibbonId == ribbonId));
        }

        public async Task<IList<ProductRibbonMappingEntity>> GetMappingsByProductIdAsync(int productId)
        {
            return await _productRibbonMappingRepository.GetAllAsync(query =>
                query.Where(x => x.ProductId == productId));
        }

        public async Task InsertMappingAsync(ProductRibbonMappingEntity mapping)
        {
            await _productRibbonMappingRepository.InsertAsync(mapping);
            await _staticCacheManager.RemoveByPrefixAsync(ProductRibbonDefaults.ProductRibbonPrefixCacheKey);
        }

        public async Task DeleteMappingAsync(ProductRibbonMappingEntity mapping)
        {
            await _productRibbonMappingRepository.DeleteAsync(mapping);
            await _staticCacheManager.RemoveByPrefixAsync(ProductRibbonDefaults.ProductRibbonPrefixCacheKey);
        }

        public async Task<ProductRibbonEntity> GetActiveRibbonByProductIdAsync(int productId)
        {
            if (productId <= 0)
                return null;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            ProductRibbonDefaults.ProductRibbonByProductIdCacheKey,
            new object[] { productId });

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var mapping = (await _productRibbonMappingRepository.GetAllAsync(query =>
                    query.Where(x => x.ProductId == productId)))
                    .FirstOrDefault();

                if (mapping == null)
                    return null;

                var ribbon = await _productRibbonRepository.GetByIdAsync(mapping.ProductRibbonId);

                if (ribbon == null || !ribbon.IsActive)
                    return null;

                return ribbon;
            });
        }
    }
}