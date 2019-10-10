using Microsoft.Extensions.Caching.Memory;

namespace CoreWebApp.Api.ApiConroller
{
    public class MyMemoryCache
    {
        public MemoryCache Cache { get; set; }
        public MyMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
    }
}