
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubWebhookService
{
    public class Program
    {
        static AgentConfigurations configurations = new();
        static Dictionary<string, AgentConfiguration> configurationDictionary = new Dictionary<string, AgentConfiguration>(StringComparer.OrdinalIgnoreCase);

        public static void Main(string[] args)
        {
            JsonSerializer serializer = new JsonSerializer();
            configurations = serializer.Deserialize<AgentConfigurations>(new JsonTextReader(new StreamReader("configuration.json")));
            foreach (AgentConfiguration c in configurations.Configurations)
            {
                configurationDictionary[c.Repo] = c;
            }

            using (var host = WebHost.Start(configurations.Host, ProcessWebRequest))
            {
                Console.WriteLine("Use Ctrl-C to shutdown the host...");
                host.WaitForShutdown();
            }
        }

        private static async Task ProcessWebRequest(HttpContext app)
        {
            string pushEvent = "push";
            StreamReader sr = new StreamReader(app.Request.Body);
            string eventType = (string)app.Request.Headers["x-GitHub-Event"];
            if (eventType != null && eventType.Equals(pushEvent))
            {
                string body = await sr.ReadToEndAsync();
                JObject j = JObject.Parse(body);
                string repositoryUpdated = (string)j["repository"]["full_name"];
                if (!configurationDictionary.ContainsKey(repositoryUpdated))
                {
                    await app.Response.WriteAsync($"Github push for untracked repo: {repositoryUpdated}");
                    return;
                }

                AgentConfiguration config = configurationDictionary[repositoryUpdated];

                string branchUpdated = (string)j["ref"];
                if (branchUpdated != null && branchUpdated.Equals(config.Branch, StringComparison.OrdinalIgnoreCase))
                {
                    await app.Response.WriteAsync($"Branch {branchUpdated} pushed new changes. Updating.");
                    UpdateService(config);
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

        private static void UpdateService(AgentConfiguration c)
        {
            foreach (string command in c.UpdateCommands)
            {
                ExecuteCommandSync(command, c);
            }
        }

        public static void ExecuteCommandSync(object command, AgentConfiguration c)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo(c.Shell ?? "cmd", "/c " + command);

                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception)
            {
            }
        }
    }
}