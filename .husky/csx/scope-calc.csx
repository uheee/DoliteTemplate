#nullable enable

using System.IO;

const string scopePrefix = "DoliteTemplate.";
const string srcRoot = "src";
var srcPath = srcRoot.Split('/');

var scopeRedefinition = new Dictionary<string, string>
{
    { "BcsEvolution.sln", "src" },
    { "README.md", "doc" }
};

bool GetScope(IEnumerable<string> files, out string? scope)
{
    scope = null;
    string[] parentDir;
    try
    {
        parentDir = GetParentPath(files);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return false;
    }

    if (IsSrcContent(parentDir))
    {
        parentDir = parentDir.Skip(srcPath!.Length).ToArray();
        if (parentDir.Any())
        {
            var projectName = parentDir.First();
            if (projectName.StartsWith(scopePrefix))
            {
                projectName = projectName.Substring(scopePrefix.Length);
            }
            parentDir = new[] {projectName}.Concat(parentDir.Skip(1)).ToArray();
        }

        scope = $"${string.Join('.', parentDir)}".ToLower();
        return true;
    }
    scope = string.Join('/', parentDir).ToLower();
    return true;
}

string[] GetParentPath(IEnumerable<string> files)
{
    return files
        .Select(file => scopeRedefinition.TryGetValue(file, out var dir) ? Path.Combine(dir, file) : file)
        .Select(Path.GetDirectoryName)
        .Select(file => file!.Replace(@"\", "/").Split('/'))
        .Aggregate((a, b) =>
        {
            if (IsSrcContent(a) ^ IsSrcContent(b))
                throw new Exception(
                    "These files are not all 'src' or 'non-src', separate them with different commits");
            return a.Intersect(b).ToArray();
        });
}

bool IsSrcContent(string[] path)
{
    return string.Join('/', path).StartsWith(srcRoot);
}