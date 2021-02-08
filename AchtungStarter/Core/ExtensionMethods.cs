using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AchtungStarter.Core
{
    public static class ExtensionMethods
    {
        public static string GetSetting(this string? value, int debugId)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"A required setting cannot be found ({debugId}).");
            }

            return value;
        }

        public static async Task RequireSuccessStatusCode(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string message;

                try
                {
                    message = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    message = $"Exception thrown while reading response content: {ex}";
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "(none)";
                }

                message = message.Length < 200 ? message : message[0..197] + "...";
                throw new HttpRequestException($"Got response {response.StatusCode} and message '{message}'.");
            }
        }
    }
}
