using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace com.aboersch.PostProxy.Function
{
    public static class PostProxy
    {
        [FunctionName("PostProxy")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var forwardingRequest = new ForwardingRequest(req.RequestUri);
            using (var client = new HttpClient())
            {
                var resp = await client.PostAsync(forwardingRequest.ForwardingUrl,
                    new StringContent(forwardingRequest.JsonContent));
                return req.CreateResponse(HttpStatusCode.OK, await resp.Content.ReadAsStringAsync());
            }
        }
    }
}