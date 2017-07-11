using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Combinatorics.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace com.aboersch.PostProxy.Tests
{
    [TestClass]
    public class ForwardingRequestTest
    {
        const string BaseAddress = "http://web.address/route";

        static readonly Parameter StringParam = new Parameter(nameof(StringParam), "string");
        static readonly Parameter BoolParam = new Parameter(nameof(BoolParam), true, "true");
        static readonly Parameter NumberParam = new Parameter(nameof(NumberParam), 1);

        const string ForwardingUrl = "http://forward.url/forwardroute";
        //$"url={Uri.EscapeDataString(FORWARDING_URL)}"
        static readonly Parameter ForwardingUrlParam = new Parameter("url", "=http%3A%2F%2Fforward.url%2Fforwardroute");

        private Uri CreateRequestUrl(params Parameter[] queryParameters)
        {
            return new Uri(CreateUrl(BaseAddress, queryParameters));
        }

        private string GetResultUrl(params Parameter[] queryParameters)
        {
            return CreateUrl(ForwardingUrl, queryParameters);
        }

        private string CreateUrl(string baseUrl, params Parameter[] queryParameters)
        {
            return $"{baseUrl}{(queryParameters.Any() ? "?" : null) }{string.Join("&", queryParameters.Select(p => p.QueryParameter))}";
        }

        private void NoForwardingUrl(params Parameter[] queryParameters)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new ForwardingRequest(CreateRequestUrl(queryParameters));
                Assert.Fail("Should throw an exception");
            }
            catch
            {
                // ignored
            }
        }

        static readonly Parameter[] ForwardingArray = { ForwardingUrlParam };

        private void ValidForwardingUrl(params Parameter[] queryParameters)
        {
            var forwardingRequest = new ForwardingRequest(CreateRequestUrl(queryParameters));
            Assert.AreEqual(
                forwardingRequest.ForwardingUrl,
                GetResultUrl(queryParameters.Except(ForwardingArray).ToArray()));
            var jsonToken = JToken.Parse(forwardingRequest.JsonContent);

            foreach (var queryParameter in queryParameters)
            {
                if (queryParameter == ForwardingUrlParam)
                    continue;
                var value = ((JValue)jsonToken[queryParameter.Key]).Value;
                var compValue = queryParameter.RawValue;
                if (compValue is int)
                {
                    compValue = Convert.ToInt64(compValue);
                }
                Assert.AreEqual(value, compValue);
            }
        }

        [TestMethod]
        public void NoForwardingUrlTest()
        {
            NoForwardingUrl(StringParam);
        }

        [TestMethod]
        public void ValidForwardingUrlTest()
        {
            ValidForwardingUrl(ForwardingUrlParam, StringParam, BoolParam, NumberParam);
        }

        [TestMethod]
        public void ParameterPermutationsTest()
        {
            var paramList = new[] { StringParam, BoolParam, NumberParam, ForwardingUrlParam };
            for (int i = 1; i <= paramList.Length; i++)
            {
                var variations = new Variations<Parameter>(paramList, i, GenerateOption.WithoutRepetition);
                foreach (var variation in variations)
                {
                    var shouldFail = !variation.Contains(ForwardingUrlParam);

                    var queryParameters = variation.ToArray();
                    if (shouldFail)
                        NoForwardingUrl(queryParameters);
                    else
                        ValidForwardingUrl(queryParameters);

                }
            }
        }
    }
}
