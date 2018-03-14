using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler.PageUrlFilters
{
    public class UniquePageUrlsFilter : IPageUrlFilter
    {
        private ConcurrentDictionary<string, byte> _previousUrls = new ConcurrentDictionary<string, byte>();
        
        public async Task<IEnumerable<PageUrl>> FilterAsync(IEnumerable<PageUrl> urls)
        {
            return urls.Where(m => _previousUrls.TryAdd(m.CurrentUrl, 0));
        }
    }
}