using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSite.Service.DataEntities
{
    public class Portfolio
    {
        public string UserName { get; set; }

        public List<RepositoryDetails> Repositories { get; set; }
    }
}
