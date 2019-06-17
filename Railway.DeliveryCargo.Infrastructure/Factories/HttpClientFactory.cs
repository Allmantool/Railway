using System;
using System.Net.Http;
using Railway.DeliveryCargo.Infrastructure.Core.Consts;
using Railway.DeliveryCargo.Infrastructure.Factories.Interfaces;

namespace Railway.DeliveryCargo.Infrastructure.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly string _correlationToken;

        public HttpClientFactory(string correlationToken)
        {
            _correlationToken = correlationToken;
        }

        public HttpClient Create(Uri uri)
        {
            var client = new HttpClient
            {
                BaseAddress = uri
            };

            client
                .DefaultRequestHeaders
                .Add(HttpHeaders.CorrelationTokenKey, _correlationToken);

            return client;
        }
    }
}
