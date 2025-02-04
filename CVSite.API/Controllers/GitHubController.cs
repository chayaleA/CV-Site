using Microsoft.AspNetCore.Mvc;
using CVSite.Service;
using System.Linq;
using System.Threading.Tasks;

namespace CVSite.API.Controllers
{
    [ApiController]
    [Route("api/github")]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("followers/{username}")]
        public async Task<IActionResult> GetUserFollowers(string username)
        {
            var followers = await _gitHubService.GetUserFollowersAsync(username);
            return Ok(followers);
        }

        [HttpGet("portfolio")]
        public async Task<IActionResult> GetPortfolio()
        {
            var repos = await _gitHubService.GetUserPortfolio();
            return Ok(repos);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchRepositories([FromQuery] string repositoryName = "", [FromQuery] string language = "", [FromQuery] string user = "")
        {
            try
            {
                var result = await _gitHubService.SearchRepositoriesAsync(repositoryName, language, user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
