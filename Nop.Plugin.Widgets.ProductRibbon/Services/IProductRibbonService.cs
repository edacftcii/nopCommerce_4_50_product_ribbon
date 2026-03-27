using System.Collections.Generic;
using System.Threading.Tasks;
using ProductRibbonEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbon;
using ProductRibbonMappingEntity = Nop.Plugin.Widgets.ProductRibbon.Domain.ProductRibbonMapping;

namespace Nop.Plugin.Widgets.ProductRibbon.Services
{
    public interface IProductRibbonService
    {
        Task<IList<ProductRibbonEntity>> GetAllRibbonsAsync();
        Task<ProductRibbonEntity> GetRibbonByIdAsync(int ribbonId);
        Task InsertRibbonAsync(ProductRibbonEntity ribbon);
        Task UpdateRibbonAsync(ProductRibbonEntity ribbon);
        Task DeleteRibbonAsync(ProductRibbonEntity ribbon);

        Task<IList<ProductRibbonMappingEntity>> GetMappingsByRibbonIdAsync(int ribbonId);
        Task<IList<ProductRibbonMappingEntity>> GetMappingsByProductIdAsync(int productId);
        Task InsertMappingAsync(ProductRibbonMappingEntity mapping);
        Task DeleteMappingAsync(ProductRibbonMappingEntity mapping);

        Task<ProductRibbonEntity> GetActiveRibbonByProductIdAsync(int productId);
    }
}