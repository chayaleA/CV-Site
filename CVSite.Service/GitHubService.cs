using CVSite.Service.DataEntities;
using Microsoft.Extensions.Options;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVSite.Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _client = new GitHubClient(new ProductHeaderValue("my-github-app"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<int> GetUserFollowersAsync(string userName)
        {
            var user = await _client.User.Get(userName);
            return user.Followers;
        }

        public async Task<Portfolio> GetUserPortfolio()
        {
            var userRepositories = await _client.Repository.GetAllForCurrent();

            var repositoryDetails = new List<RepositoryDetails>();

            foreach (var repo in userRepositories)
            {
                var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                repositoryDetails.Add(new RepositoryDetails
                {
                    Name = repo.Name,
                    Language = repo.Language,
                    LastCommitDate = repo.UpdatedAt,
                    Stars = repo.StargazersCount,
                    PullRequests = pullRequests.Count,
                    HtmlUrl = repo.HtmlUrl
                });
            }

            return new Portfolio
            {
                UserName = _options.UserName,
                Repositories = repositoryDetails
            };
        }

        public async Task<List<RepositoryDetails>> SearchRepositoriesAsync(string repositoryName = "", string language = "", string user = "")
        {
            var request = string.IsNullOrEmpty(repositoryName) ? new SearchRepositoriesRequest() : new SearchRepositoriesRequest(repositoryName);

            if (!string.IsNullOrEmpty(language))
            {
                if (Enum.TryParse<Language>(language, true, out var parsedLanguage))
                {
                    request.Language = parsedLanguage;
                }
                else
                {
                    Console.WriteLine($"Invalid language: {language}");
                }
            }

            if (!string.IsNullOrEmpty(user))
            {
                request.User = user;
            }

            try
            {
                var result = (await _client.Search.SearchRepo(request)).Items;

                if (!string.IsNullOrEmpty(repositoryName))
                    result.Where(repo => string.Equals(repo.Name, repositoryName, StringComparison.OrdinalIgnoreCase));

                var repositoryDetailsList = new List<RepositoryDetails>();

                foreach (var repo in result)
                {
                    var pullRequests = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                    repositoryDetailsList.Add(new RepositoryDetails
                    {
                        Name = repo.Name,
                        Language = repo.Language,
                        LastCommitDate = repo.UpdatedAt,
                        Stars = repo.StargazersCount,
                        PullRequests = pullRequests.Count,
                        HtmlUrl = repo.HtmlUrl
                    });
                }

                return repositoryDetailsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching repositories: {ex.Message}");
                return new List<RepositoryDetails>();
            }
        }

    }
}
