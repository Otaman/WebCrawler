using System.Net.Http;
using System.Threading.Tasks;

namespace WebCrawler
{
    public interface IPageProcessor
    {
        Task ProcessSuccessfulAsync(PageUrl url, HttpResponseMessage response);
        Task ProcessFailedAsync(PageUrl url, HttpRequestException e);
    }
}