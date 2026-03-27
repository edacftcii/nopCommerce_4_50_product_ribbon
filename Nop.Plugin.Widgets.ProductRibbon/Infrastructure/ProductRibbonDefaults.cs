using Nop.Core.Caching;

namespace Nop.Plugin.Widgets.ProductRibbon.Infrastructure
{
    public static class ProductRibbonDefaults
    {
        public static CacheKey ProductRibbonByProductIdCacheKey =>
            new CacheKey("Nop.productribbon.byproduct.{0}", ProductRibbonPrefixCacheKey);

        public static CacheKey ProductRibbonAllCacheKey =>
            new CacheKey("Nop.productribbon.all", ProductRibbonPrefixCacheKey);

        public static string ProductRibbonPrefixCacheKey => "Nop.productribbon.";
    }
}