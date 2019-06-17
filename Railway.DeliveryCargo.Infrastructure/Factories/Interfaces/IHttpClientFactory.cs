using System;
using System.Net.Http;

namespace Railway.DeliveryCargo.Infrastructure.Factories.Interfaces
{
    public interface IHttpClientFactory
    {
        HttpClient Create(Uri uri);
    }
}
