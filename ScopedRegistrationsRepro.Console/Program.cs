using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ScopedRegistrationsRepro.Console
{
    class Program
    {
        private static Func<string, HttpRequestMessage> requestBuilder;
        static async Task Main(string[] args)
        {
            var url = "http://localhost:7071/api/account/read";

            requestBuilder = (header) =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, url);
                    message.Headers.Add("x-custom-test-header", header);
                    message.Headers.Add("x-functions-key", "BImbYp4Nx1nCkXdP5qoYoxekq0Aabthu0Y4y42DSvPcFotlrimTTIA==");
                return message ;
            };

            var range = Enumerable.Range(0, 100)
                .Select(a => new { index=a, header= $"Header for request {a}"})
                .Select(a => CallAndPrint(a.index, a.header))
                .ToArray();

            await Task.WhenAll(range);

            System.Console.WriteLine($"Done");
            System.Console.ReadLine();
        }

        private static async Task CallAndPrint(int i, string message)
        {
            var request = requestBuilder.Invoke(message);
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            System.Console.WriteLine($"{response.StatusCode} - response for request {i} \r\n {body}");
        }
    }
}
