using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubWebhookService
{
    public class AgentConfiguration
    {
        public string Host { get; set; }
        public string Branch { get; set; }
        public List<string> UpdateCommands { get; set; }
    }
}
