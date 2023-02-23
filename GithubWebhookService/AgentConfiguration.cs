using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubWebhookService
{
    public class AgentConfiguration
    {
        public string Repo { get; set; }
        public string Branch { get; set; }
        public List<string> UpdateCommands { get; set; }
        public string Shell { get; set; }
    }

    public class AgentConfigurations
    {
        public string Host { get; set; }
        public List<AgentConfiguration> Configurations { get; set; }
    }
}
