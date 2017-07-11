using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.aboersch.PostProxy
{
    public class ForwardingRequest
    {
        public string ForwardingUrl { get; }
        public string JsonContent { get; }
        public ForwardingRequest(Uri requestUri)
        {
            string url = null;
            var forwardingParams = new StringBuilder();
            var json = new JObject();
            foreach (var requestParameter in requestUri.Query.TrimStart('?')
                .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => new RequestParameter(parameter)))
            {
                if (requestParameter.Key.Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    url = Uri.UnescapeDataString(requestParameter.Value);
                    continue;
                }
                forwardingParams.AppendFormat("{0}{1}={2}", forwardingParams.Length == 0 ? '?' : '&',
                    requestParameter.Key, requestParameter.Value);
                json.Add(requestParameter.Key, requestParameter.JToken);
            }
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The 'url' query parameter is not optional");
            ForwardingUrl = url + forwardingParams;
            
            JsonContent = json.ToString(Formatting.None);
        }
    }
}
