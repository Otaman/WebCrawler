using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCrawler.PageUrlExtractors
{
    internal class HtmlAgilityPackUrlExtractor : IPageUrlExtractor
    {
        public Task<IEnumerable<PageUrl>> ExtractAsync(PageUrl url, HttpResponseMessage response)
        {
            throw new System.NotImplementedException();
        }
    }
}