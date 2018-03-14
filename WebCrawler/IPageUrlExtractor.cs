using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCrawler
{
    public interface IPageUrlExtractor
    {
        Task<IEnumerable<PageUrl>> ExtractAsync(PageUrl url, HttpResponseMessage response);
    }
}