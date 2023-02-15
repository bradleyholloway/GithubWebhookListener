
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
            string mainBranch = "refs/heads/main";
            string pushEvent = "push";
            StreamReader sr = new StreamReader(app.Request.Body);
            string eventType = (string)app.Request.Headers["x-GitHub-Event"];
            if (eventType != null && eventType.Equals(pushEvent))
            {
                string body = await sr.ReadToEndAsync();
                JObject j = JObject.Parse(body);
                string branchUpdated = (string)j["ref"];
                if (branchUpdated != null && branchUpdated.Equals(mainBranch))
                {
                    await app.Response.WriteAsync($"Branch {branchUpdated} pushed new changes.");
                }
                else
                {
                    await app.Response.WriteAsync($"Github push for untracked branch: {branchUpdated}");
                }
            }
            else
            {
                await app.Response.WriteAsync($"No Github Event, or nonpush event: {pushEvent}");
            }
        }
    }
}