using System.Net;
using System.Net.Http.Json;

namespace Slottet.Client.Pages.AdminPages
{
    internal sealed record AdminLoadResult<T>(T? Value, string? ErrorMessage)
    {
        public bool Failed => !string.IsNullOrWhiteSpace(ErrorMessage);
    }

    internal static class AdminHttp
    {
        private const string ForbiddenMessage = "Du har ikke adgang til denne side.";
        private const string GenericMessage = "Kunne ikke loade data. Prøv venligst igen senere. - AdminHttp";

        public static async Task<AdminLoadResult<T>> GetJsonAsync<T>(HttpClient httpClient, string requestUri)
        {
            try
            {
                using var response = await httpClient.GetAsync(requestUri);

                if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                {
                    return new AdminLoadResult<T>(default, ForbiddenMessage);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new AdminLoadResult<T>(default, GenericMessage);
                }

                var value = await response.Content.ReadFromJsonAsync<T>();
                return new AdminLoadResult<T>(value, null);
            }
            catch
            {
                return new AdminLoadResult<T>(default, GenericMessage);
            }
        }
    }
}
