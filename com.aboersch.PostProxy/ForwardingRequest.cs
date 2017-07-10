using System;
using System.Linq;
using System.Text;

namespace com.aboersch.PostProxy
{
    public class ForwardingRequest
    {
        public string Url { get; }
        public string Content { get; }
        public ForwardingRequest(Uri requestUri)
        {
            string url = null;
            var forwardingParams = new StringBuilder();
            var forwardingContent = new StringBuilder("{");
            foreach (var requestParameter in requestUri.Query.TrimStart('?')
                .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (requestParameter[0].Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    url = Uri.UnescapeDataString(requestParameter[1]);
                    continue;
                }
                forwardingParams.AppendFormat("{0}{1}={2}", forwardingParams.Length == 0 ? '?' : '&', requestParameter[0], requestParameter[1]);
                forwardingContent.AppendFormat("{0}\"{1}\":{2}", forwardingContent.Length == 1 ? null : ",", requestParameter[0], ParseData(requestParameter[1]));
            }
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The 'url' query parameter is not optional");
            Url = url + forwardingParams;

            forwardingContent.Append('}');
            Content = forwardingContent.ToString();
        }

        private static string ParseData(string data)
        {
            try
            {
                return Newtonsoft.Json.Linq.JToken.Parse(data).ToString();
            }
            catch
            {
                return string.Format("\"{0}\"", data);
            }
        }
    }
}
