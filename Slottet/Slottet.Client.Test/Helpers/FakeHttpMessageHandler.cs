using Azure.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Slottet.Client.Test.Helpers {
    public class FakeHttpMessageHandler : HttpMessageHandler {
        public List<HttpRequestMessage> Requests { get; } = new();

        private readonly Dictionary<(HttpMethod Method, string Path), Func<HttpRequestMessage, HttpResponseMessage>> _routes = new();

        public void AddJson<T>(HttpMethod method, string path, T body, HttpStatusCode status = HttpStatusCode.OK) {
            _routes[(method, path)] = _ => new HttpResponseMessage(status) {
                Content = JsonContent.Create(body)
            };
        }

        public void AddStatus(HttpMethod method, string path, HttpStatusCode status) {
            _routes[(method, path)] = _ => new HttpResponseMessage(status);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            Requests.Add(request);

            var path = request.RequestUri!.PathAndQuery.TrimStart('/');

            if(_routes.TryGetValue((request.Method, path), out var responseFactory)) {
                return Task.FromResult(responseFactory(request));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
