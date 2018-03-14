using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebCrawler.PageUrlExtractors
{
    internal class HtmlAgilityPackUrlExtractor : IPageUrlExtractor
    {
        public async Task<IEnumerable<PageUrl>> ExtractAsync(PageUrl url, HttpResponseMessage response)
        {
            var doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync());
            var links = doc.DocumentNode.Descendants("a")
                .Select(a => a.GetAttributeValue("href", null))
                .Where(u => !string.IsNullOrEmpty(u));
            
            var baseUrl = new Uri(url.CurrentUrl);
            var authority = baseUrl.GetLeftPart(UriPartial.Authority);

            return links.Select(link =>
            {
                var uri = new Uri(new Uri(authority), new Uri(link));
                var preparedUrl = uri.ToString();
                return new PageUrl(url.Depth + 1, url.CurrentUrl, preparedUrl);
            });
        }
    }
}