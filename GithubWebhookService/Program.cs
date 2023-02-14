
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GithubWebhookService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var host = WebHost.Start("http://localhost", ProcessWebRequest))
            {
                Console.WriteLine("Use Ctrl-C to shutdown the host...");
                host.WaitForShutdown();
            }
        }

        public static async Task ProcessWebRequest(HttpContext app)
        {
            await app.Response.WriteAsync("Hello, World!");
        }
    }
}