using CVSite.Service.DataEntities;
using Octokit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CVSite.Service
{
    public interface IGitHubService
    {
        Task<int> GetUserFollowersAsync(string userName);

        Task<Portfolio> GetUserPortfolio();

        Task<List<RepositoryDetails>> SearchRepositoriesAsync(string repositoryName, string language, string user);
    }
}
