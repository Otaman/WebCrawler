using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.PageUrlExtractors;

namespace WebCrawler
{
    public class Crawler
    {
        private static readonly HttpClient Client = new HttpClient();
        
        private IPageProcessor Processor { get; }
        private IEnumerable<IPageUrlFilter> UrlFilters { get; }
        private IPageUrlExtractor UrlExtractor { get; }

        public Crawler(IPageProcessor processor, IEnumerable<IPageUrlFilter> urlFilters = null, 
            IPageUrlExtractor urlExtractor = null)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor));
            UrlFilters = urlFilters ?? new IPageUrlFilter[] {};
            UrlExtractor = urlExtractor ?? new HtmlAgilityPackUrlExtractor();
        }
        
        public async Task StartAsync(string rootPageUrl, CancellationToken cancellationToken, int? depht = null, 
            int? maxConcurrentConnections = null)
        {
            await StartAsync(new PageUrl(0, null, rootPageUrl), cancellationToken, depht, maxConcurrentConnections);
        }

        public async Task StartAsync(PageUrl rootPageUrl, CancellationToken cancellationToken, int? depht = null,
            int? maxConcurrentConnections = null)
        {
            var bufferBlock = new BufferBlock<PageUrl>(new DataflowBlockOptions()
            {
                CancellationToken = cancellationToken
            });
            var actionBlock = new ActionBlock<PageUrl>(p => CrawlPage(p, cancellationToken, depht, bufferBlock), 
                new ExecutionDataflowBlockOptions()
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = maxConcurrentConnections ?? DataflowBlockOptions.Unbounded
                });
            bufferBlock.LinkTo(actionBlock, new DataflowLinkOptions() {PropagateCompletion = true});
            
            bufferBlock.Post(rootPageUrl);
            
            await bufferBlock.Completion;
        }

        private async Task CrawlPage(PageUrl pageUrl, CancellationToken cancellationToken, int? depht,
            BufferBlock<PageUrl> bufferBlock)
        {
            HttpResponseMessage response;
            try
            {
                response = await Client.GetAsync(pageUrl.CurrentUrl, cancellationToken);
            }
            catch (HttpRequestException e)
            {
                await Processor.ProcessFailedAsync(pageUrl, e);
                return;
            }
            
            await Processor.ProcessSuccessfulAsync(pageUrl, response);
                
            var extractedUrls = await ExtractUrls(response, pageUrl);
            var filteredUrls = await FilterUrls(extractedUrls);
            filteredUrls = FilterByDepth(depht, filteredUrls);

            foreach (var filteredUrl in filteredUrls)
            {
                await bufferBlock.SendAsync(filteredUrl, cancellationToken);
            }
        }

        private static IEnumerable<PageUrl> FilterByDepth(int? depht, IEnumerable<PageUrl> filteredUrls)
        {
            if (depht.HasValue)
            {
                filteredUrls = filteredUrls
                    .Where(m => m.Depth <= depht.Value);
            }

            return filteredUrls;
        }

        private async Task<IEnumerable<PageUrl>> FilterUrls(IEnumerable<PageUrl> urls)
        {
            var result = urls;
            foreach (var urlFilter in UrlFilters)
            {
                result = await urlFilter.FilterAsync(result);
            }

            return result;
        }

        private Task<IEnumerable<PageUrl>> ExtractUrls(HttpResponseMessage response, PageUrl pageUrl)
        {
            return UrlExtractor.ExtractAsync(pageUrl, response);
        }
    }
}