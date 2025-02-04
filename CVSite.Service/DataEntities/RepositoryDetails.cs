using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSite.Service.DataEntities
{
    public class RepositoryDetails
    {
        public string Name {  get; set; }

        public string Language { get; set; }

        public DateTimeOffset LastCommitDate { get; set; }

        public int Stars { get; set; }

        public int PullRequests { get; set; }

        public string HtmlUrl { get; set; }
    }
}
