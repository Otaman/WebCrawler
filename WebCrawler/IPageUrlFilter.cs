using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebCrawler
{
    public interface IPageUrlFilter
    {
        Task<IEnumerable<PageUrl>> FilterAsync(IEnumerable<PageUrl> urls);
    }
}