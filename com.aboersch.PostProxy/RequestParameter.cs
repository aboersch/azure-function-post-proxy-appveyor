using System;
using Newtonsoft.Json.Linq;

namespace com.aboersch.PostProxy
{
    class RequestParameter
    {
        public string Key { get; }
        public string Value { get; }

        public JToken JToken
        {
            get
            {
                try
                {
                    return JToken.Parse(Value);
                }
                catch
                {
                    return new JValue(Value);
                }
            }
        }

        public RequestParameter(string parameter)
        {
            var split = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            Key = split[0];
            Value = split[1];
        }
    }
}
