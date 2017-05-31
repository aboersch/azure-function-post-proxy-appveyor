using System;
using System.Linq;
using System.Text;

namespace com.aboersch.PostProxy
{
    public static class ForwardingRequestBuilder
    {
        public static string CreateUrl(Uri requestUri)
        {
            string url = null;
            var sb = new StringBuilder();
            foreach(var requestParameter in requestUri.Query.TrimStart('?')
                        .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (requestParameter[0].Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    url = Uri.UnescapeDataString(requestParameter[1]);
                    continue;
                }
                sb.AppendFormat("{0}{1}={2}", sb.Length == 0 ? '?' : '&', requestParameter[0], requestParameter[1]);
            }
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("The 'url' query parameter is not optional");
            return url + sb.ToString();
        }
    }
}
