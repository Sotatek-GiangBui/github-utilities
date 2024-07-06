using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace GithubUtils.Service;

public class GithubService
{
    private static readonly HttpClient client = new HttpClient();
    private string url;
    private string repo;
    private string branch;
    private string token;
    private string owner;

    public GithubService(string owner, string repo, string branch, string token)
    {
        this.repo = repo;
        this.branch = branch;
        this.token = token;
        this.owner = owner;
        url = $"https://api.github.com/repos/{owner}/{repo}";
        client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
    }

    public async Task<bool> CheckIfRepositoryExists()
    {
        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var repoData = JObject.Parse(content);
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Not found
                return false;
            }
            else
            {
                // Handle other statuses if needed
                throw new Exception($"Unexpected status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return false;
        }
    }

    public async Task<bool> DownloadRepoAsAnArchive(string localPath)
    {
        FileHelper.EnsureDirectoryExists(localPath);

        string downloadUrl = $"https://api.github.com/repos/{owner}/{repo}/zipball/{branch}";
        var response = await client.GetAsync(downloadUrl);
        response.EnsureSuccessStatusCode();

        var contentStream = await response.Content.ReadAsStreamAsync();
        var filePath = $"{localPath}/{repo}-{branch}.zip";

        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            await contentStream.CopyToAsync(fileStream);
        }

        return true;
    }


}
