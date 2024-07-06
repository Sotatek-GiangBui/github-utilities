using GithubUtils.Service;

namespace GithubUtils;

public class Program
{
    private static string githubToken { get; } = "";
    private static string owner = "lodash"; // github username
    private static string repo = "lodash"; // the repo name
    private static string branch = "main";
    public static async Task Main()
    {
        Console.WriteLine("Starting");
        Console.WriteLine("");

        var githubService = new GithubService(owner, repo, branch, githubToken);

        // check exist
        var exists = await githubService.CheckIfRepositoryExists();
        if (!exists)
        {
            Console.WriteLine("The repository does not exist.");
            return;
        }

        // load ziped file to local
        var path = $"{Directory.GetCurrentDirectory()}/temp";
        await githubService.DownloadRepoAsAnArchive(path);
        Console.WriteLine($"Downloaded file from github to local path");

        // unzip the file
        var result = await FileHelper.UnzipAllFilesAsync(path);
        if (!result)
        {
            Console.WriteLine("Unzip operation is failed");
        }
        Console.WriteLine($"Unzip operation completed");

        // get all the ts and js files in tmp folder
        var files = FileHelper.GetFilesByExtensions(path);
        Console.WriteLine($"Filtered files with specific extensions: {files.Count()}");

        // Count letter frequencies
        Dictionary<char, int> letterCount = FileHelper.CountLettersInFiles(files);

        // Sort by frequency in descending order
        var sortedLetters = letterCount.OrderByDescending(pair => pair.Value);

        // Display results
        Console.WriteLine("Letter frequencies (descending order):");
        foreach (var pair in sortedLetters)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Console.ReadKey();
    }
}
