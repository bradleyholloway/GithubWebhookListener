
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace GithubWebhookService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var host = WebHost.Start("http://shadowaid.com:8080", ProcessWebRequest))
            {
                Console.WriteLine("Use Ctrl-C to shutdown the host...");
                host.WaitForShutdown();
            }
        }

        public static async Task ProcessWebRequest(HttpContext app)
        {
            StreamReader sr = new StreamReader(app.Request.Body);
            string body = await sr.ReadToEndAsync();
            Console.WriteLine(body);
            JObject j = JObject.Parse(body);
            await app.Response.WriteAsync("Hello, World!");
        }
    }
}