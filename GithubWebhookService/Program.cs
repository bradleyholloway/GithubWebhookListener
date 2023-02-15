
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubWebhookService
{
    public class Program
    {
        static AgentConfiguration configuration = new();

        public static void Main(string[] args)
        {
            JsonSerializer serializer = new JsonSerializer();
            configuration = serializer.Deserialize<AgentConfiguration>(new JsonTextReader(new StreamReader("configuration.json")));
            using (var host = WebHost.Start(configuration.Host, ProcessWebRequest))
            {
                Console.WriteLine("Use Ctrl-C to shutdown the host...");
                host.WaitForShutdown();
            }
        }

        private static async Task ProcessWebRequest(HttpContext app)
        {
            string mainBranch = configuration.Branch;
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
                    UpdateService();
                }
                else
                {
                    await app.Response.WriteAsync($"Github push for untracked branch: {branchUpdated}");
                }
            }
            else
            {
                await app.Response.WriteAsync($"No Github Event, or nonpush event: {eventType}");
            }
        }

        private static void UpdateService()
        {
            foreach (string c in configuration.UpdateCommands)
            {
                ExecuteCommandSync(c);
            }
        }

        public static void ExecuteCommandSync(object command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
            }
            catch (Exception)
            {
            }
        }
    }
}