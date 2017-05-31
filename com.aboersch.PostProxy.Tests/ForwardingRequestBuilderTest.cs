using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Combinatorics.Collections;
using System.Linq;

namespace com.aboersch.PostProxy.Tests
{
    [TestClass]
    public class ForwardingRequestBuilderTest
    {
        const string BASE_ADDRESS = "http://web.address/route";
        const string PARAM_1 = "param1=param1value";
        const string PARAM_2 = "param2=param2value";
        const string FORWARDING_URL = "http://forward.url/forwardroute";
        //$"url={Uri.EscapeDataString(FORWARDING_URL)}"
        const string PARAM_FORWARDING_URL = "url=http%3A%2F%2Fforward.url%2Fforwardroute";

        private Uri CreateRequestUrl(params string[] queryParameters)
        {
            return new Uri(CreateUrl(BASE_ADDRESS, queryParameters));
        }

        private string GetResultUrl(params string[] queryParameters)
        {
            return CreateUrl(FORWARDING_URL, queryParameters);
        }

        private string CreateUrl(string baseUrl, params string[] queryParameters)
        {
            return $"{baseUrl}{(queryParameters.Any() ? "?" : null) }{string.Join("&", queryParameters)}";
        }

        private void NoForwardingUrl(params string[] queryParameters)
        {
            try
            {
                ForwardingRequestBuilder.CreateUrl(CreateRequestUrl(queryParameters));
                Assert.Fail("Should throw an exception");
            }
            catch { }
        }

        static string[] ForwardingArray = new[] { PARAM_FORWARDING_URL };

        public void ValidForwardingUrl(params string[] queryParameters)
        {
            Assert.AreEqual(
                ForwardingRequestBuilder.CreateUrl(CreateRequestUrl(queryParameters)),
                GetResultUrl(queryParameters.Except(ForwardingArray).ToArray()));
        }

        [TestMethod]
        public void NoForwardingUrlTest()
        {
            NoForwardingUrl(PARAM_1);
        }

        [TestMethod]
        public void ValidForwardingUrlTest()
        {
            ValidForwardingUrl(PARAM_FORWARDING_URL, PARAM_1);
        }

        [TestMethod]
        public void ParameterPermutationsTest()
        {
            var paramList = new List<string>(new[] { PARAM_1, PARAM_2, PARAM_FORWARDING_URL });
            for (int i = 1; i < 4; i++)
            {
                var variations = new Variations<string>(paramList, i, GenerateOption.WithoutRepetition);
                foreach (var variation in variations)
                {
                    var shouldFail = !variation.Contains(PARAM_FORWARDING_URL);

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
