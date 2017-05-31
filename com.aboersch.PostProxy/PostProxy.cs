using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace com.aboersch.PostProxy
{
    public static class PostProxy
    {
        [FunctionName("PostProxy")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var forwardingUrl = ForwardingRequestBuilder.CreateUrl(req.RequestUri);
            using (var client = new HttpClient())
            {
                var resp = await client.PostAsync(forwardingUrl, req.Content);
                return req.CreateResponse(HttpStatusCode.OK, await resp.Content.ReadAsStringAsync());
            }
        }
    }
}