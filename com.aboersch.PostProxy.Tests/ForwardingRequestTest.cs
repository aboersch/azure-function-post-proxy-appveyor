using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Combinatorics.Collections;
using System.Linq;

namespace com.aboersch.PostProxy.Tests
{
    [TestClass]
    public class ForwardingRequestTest
    {
        const string BaseAddress = "http://web.address/route";
        const string StringParam = "stringParam=string";
        const string BoolParam = "boolParam=true";
        const string NumberParam = "numberParam=1";
        const string ForwardingUrl = "http://forward.url/forwardroute";
        //$"url={Uri.EscapeDataString(FORWARDING_URL)}"
        const string ParamForwardingUrl = "url=http%3A%2F%2Fforward.url%2Fforwardroute";

        private Uri CreateRequestUrl(params string[] queryParameters)
        {
            return new Uri(CreateUrl(BaseAddress, queryParameters));
        }

        private string GetResultUrl(params string[] queryParameters)
        {
            return CreateUrl(ForwardingUrl, queryParameters);
        }

        private string CreateUrl(string baseUrl, params string[] queryParameters)
        {
            return $"{baseUrl}{(queryParameters.Any() ? "?" : null) }{string.Join("&", queryParameters)}";
        }

        private void NoForwardingUrl(params string[] queryParameters)
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

        static readonly string[] ForwardingArray = { ParamForwardingUrl };

        private void ValidForwardingUrl(params string[] queryParameters)
        {
            var forwardingRequest = new ForwardingRequest(CreateRequestUrl(queryParameters));
            Assert.AreEqual(
                forwardingRequest.Url,
                GetResultUrl(queryParameters.Except(ForwardingArray).ToArray()));
        }

        [TestMethod]
        public void NoForwardingUrlTest()
        {
            NoForwardingUrl(StringParam);
        }

        [TestMethod]
        public void ValidForwardingUrlTest()
        {
            ValidForwardingUrl(ParamForwardingUrl, StringParam, BoolParam, NumberParam);
        }

        [TestMethod]
        public void ParameterPermutationsTest()
        {
            var paramList = new[] { StringParam, BoolParam, ParamForwardingUrl };
            for (int i = 1; i <= paramList.Length; i++)
            {
                var variations = new Variations<string>(paramList, i, GenerateOption.WithoutRepetition);
                foreach (var variation in variations)
                {
                    var shouldFail = !variation.Contains(ParamForwardingUrl);

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
