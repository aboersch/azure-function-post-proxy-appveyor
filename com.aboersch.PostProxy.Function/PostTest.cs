using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;

namespace com.aboersch.PostProxy.Function
{
    public static class PostTest
    {
        [FunctionName("TestPost")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            if(string.IsNullOrEmpty(name))
                try
                {
                    // Get request body as JSON
                    var data = JObject.Parse(await req.Content.ReadAsStringAsync());
                    JToken value;
                    if (data != null && data.TryGetValue("name", StringComparison.OrdinalIgnoreCase, out value))
                        name = value.ToString();
                }
                catch
                {
                    // ignored
                }

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}