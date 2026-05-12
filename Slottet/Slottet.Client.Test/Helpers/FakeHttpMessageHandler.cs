using Azure.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Slottet.Client.Test.Helpers {
    /// <summary>
    /// Fake HTTP message handler used for client-side tests.
    /// Simulates API responses without making real HTTP requests and
    /// tracks all outgoing requests for assertion purposes.
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler {
        /// <summary>
        /// Stores all HTTP requests executed during the test run.
        /// Used to verify that expected API calls were made.
        /// </summary>
        public List<HttpRequestMessage> Requests { get; } = new();

        /// <summary>
        /// Maps HTTP method and request path combinations
        /// to mocked response factories.
        /// </summary>
        private readonly Dictionary<(HttpMethod Method, string Path), Func<HttpRequestMessage, HttpResponseMessage>> _routes = new();

        /// <summary>
        /// Registers a mocked JSON response for a specific HTTP method and path.
        /// </summary>
        /// <typeparam name="T"> Type of the response body to serialize as JSON. </typeparam>
        /// <param name="method"> HTTP method to match. </param>
        /// <param name="path"> Relative request path to match. </param>
        /// <param name="body"> Object returned as JSON response content. </param>
        /// <param name="status"> HTTP status code returned with the response. Defaults to 200 OK. </param>
        public void AddJson<T>(HttpMethod method, string path, T body, HttpStatusCode status = HttpStatusCode.OK) {
            _routes[(method, path)] = _ => new HttpResponseMessage(status) {
                Content = JsonContent.Create(body)
            };
        }

        /// <summary>
        /// Registers a mocked response that returns only an HTTP status code. Useful for endpoints where no response body is required.
        /// </summary>
        /// <param name="method"> HTTP method to match. </param>
        /// <param name="path"> Relative request path to match. </param>
        /// <param name="status"> HTTP status code returned for the request. </param>
        public void AddStatus(HttpMethod method, string path, HttpStatusCode status) {
            _routes[(method, path)] = _ => new HttpResponseMessage(status);
        }

        /// <summary>
        /// Intercepts outgoing HTTP requests and returns the configured mock response.
        /// If no matching route exists, a 404 NotFound response is returned.
        /// </summary>
        /// <param name="request"> Incoming HTTP request. </param>
        /// <param name="cancellationToken"> Token used to cancel the asynchronous operation. </param>
        /// <returns> A mocked HTTP response matching the configured route. </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            // Track all executed requests for later assertions.
            Requests.Add(request);

            // Extract the relative request path.
            var path = request.RequestUri!.PathAndQuery.TrimStart('/');

            // Return configured response if route exists.
            if (_routes.TryGetValue((request.Method, path), out var responseFactory)) {
                return Task.FromResult(responseFactory(request));
            }

            // Default fallback response for unknown routes.
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
