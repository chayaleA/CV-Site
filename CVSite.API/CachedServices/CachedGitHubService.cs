using CVSite.Service;
using CVSite.Service.DataEntities;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace CVSite.API.CachedServices
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;

        private const string UserPortfolioKey = "UserPortfolioKey";

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public Task<int> GetUserFollowersAsync(string userName)
        {
            return _gitHubService.GetUserFollowersAsync(userName);
        }

        public async Task<Portfolio> GetUserPortfolio()
        {
            if (_memoryCache.TryGetValue(UserPortfolioKey, out Portfolio portfolio))
                return portfolio;

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(10));

            portfolio = await _gitHubService.GetUserPortfolio();
            _memoryCache.Set(UserPortfolioKey, portfolio, cacheOptions);

            return portfolio;
        }

        public Task<List<RepositoryDetails>> SearchRepositoriesAsync(string repositoryName, string language, string user)
        {
            return _gitHubService.SearchRepositoriesAsync(repositoryName, language, user);
        }
    }
}
