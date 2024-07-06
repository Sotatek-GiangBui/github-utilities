using System.IO.Compression;

namespace GithubUtils.Service;

public static class FileHelper
{
    public static bool EnsureDirectoryExists(string localPath)
    {
        try
        {
            // Check if the directory exists
            if (!Directory.Exists(localPath))
            {
                // Create the directory if it doesn't exist
                Directory.CreateDirectory(localPath);
                Console.WriteLine($"Directory created: {localPath}");
            }
            else
            {
                Console.WriteLine($"Directory already exists: {localPath}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> UnzipAllFilesAsync(string localPath)
    {
        try
        {
            // Ensure the directory exists
            if (!Directory.Exists(localPath))
            {
                Console.WriteLine("The specified directory does not exist.");
                return false;
            }

            // Get all zip files in the directory
            var zipFiles = Directory.GetFiles(localPath, "*.zip");

            foreach (var zipFile in zipFiles)
            {
                // Determine the extraction path
                string extractPath = Path.Combine(localPath, Path.GetFileNameWithoutExtension(zipFile));

                // Unzip the file asynchronously
                await Task.Run(() => ZipFile.ExtractToDirectory(zipFile, extractPath, true));
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

    public static string[] GetFilesByExtensions(string path, string[]? extensions = null)
    {
        string[] defaultExtensions = { ".ts", ".js" };
        if (extensions == null || extensions.Length == 0)
        {
            // Use default extensions if extensions parameter is null or empty
            extensions = defaultExtensions;
        }

        try
        {
            // Check if the directory exists
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }

            // Get all files in the directory
            var allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            // Filter files by extensions
            var filteredFiles = allFiles.Where(file => extensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToArray();

            return filteredFiles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new string[0]; // Return an empty array in case of error
        }
    }

    public static Dictionary<char, int> CountLettersInFiles(string[] files)
    {
        var letterCount = new Dictionary<char, int>();

        try
        {
            foreach (var file in files)
            {
                // Read file content
                string content = File.ReadAllText(file);

                // Count letters in the file content
                foreach (char c in content)
                {
                    // Only count alphabetic letters
                    if (char.IsLetter(c))
                    {
                        char lowercaseChar = char.ToLower(c);
                        if (letterCount.ContainsKey(lowercaseChar))
                        {
                            letterCount[lowercaseChar]++;
                        }
                        else
                        {
                            letterCount[lowercaseChar] = 1;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return letterCount;
    }
}
