using GitDeleteObsoleteBranches;

using Medallion.Shell;

var spinner = new Spinner();
spinner.Start();

// Check if path is a git repository
if (!await IsGitRepository(Environment.CurrentDirectory))
    ExitWithError("Not a git repository");

// Get all local branches
var localBranchesResult = await GetCommandResult(Command.Run("git", "branch", "-a"));
var localBranches = localBranchesResult
    .Split("\n")
    .Select(x => x.Trim())
    .Where(x => !x.StartsWith("remotes") && !string.IsNullOrWhiteSpace(x))
    .ToList();

// Get local branches with non existing origins (deleted on remote)
var prunedBranchesResult = await Command.Run("git", "fetch", "--prune", "--dry-run").Task;
var prunedBranches = prunedBranchesResult.StandardError
    .Split("\n")
    .Where(x => x.StartsWith(" - "))
    .Select(x => x.Split("->").Last().Trim().Replace("origin/", string.Empty))
    .ToList();

var branchesToRemove = prunedBranches.Where(x => localBranches.Contains(x)).ToList();
spinner.Stop();

if (!branchesToRemove.Any())
    ExitWithError("No removable branches");

Console.WriteLine("This branches are going to be removed:");
foreach (var branch in branchesToRemove)
    Console.WriteLine($" - {branch}");
Console.WriteLine();

Console.WriteLine("Delete this branches? Y/N:");
var key = Console.ReadLine();
Console.WriteLine();

if (string.IsNullOrWhiteSpace(key) || !key.StartsWith("Y"))
    ExitWithError("Abandoning cleanup");

foreach (var branch in branchesToRemove)
    Console.Write(await GetCommandResult(Command.Run("git", "branch", "-D", branch)));
Console.WriteLine();

Console.WriteLine("Performing git prune");

spinner = new Spinner();
spinner.Start();

await Command.Run("git", "fetch", "--prune").Task;

spinner.Stop();

Console.WriteLine("Cleanup finished.");

async Task<bool> IsGitRepository(string path)
{
    if (string.IsNullOrEmpty(path))
        return false;

    if (!Directory.Exists(path))
        return false;

    Directory.SetCurrentDirectory(path);

    var commandOutput = await GetCommandResult(Command.Run("git", "rev-parse", "--is-inside-work-tree"));
    commandOutput = commandOutput.Replace("\n", string.Empty);
    if (commandOutput != true.ToString().ToLower())
        return false;
    return true;
}

async Task<string> GetCommandResult(Command command)
{
    var result = await command.Task;

    return result.Success ? result.StandardOutput : result.StandardError;
}

void ExitWithError(string message)
{
    spinner?.Stop();
    Console.WriteLine($"Error: {message}");
    Environment.Exit(0);
}
