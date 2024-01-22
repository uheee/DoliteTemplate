#nullable enable

using System.IO;
using System.Text;
using System.Text.Json;

bool GetScope(IEnumerable<string> files, string scopeFilename, out string? scope)
{
    scope = null;
    string[] parentDir;
    
    var scopeConfiguration = GetScopeConfiguration(scopeFilename);
    
    try
    {
        parentDir = GetParentPath(scopeConfiguration, files);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return false;
    }
    
    if (CheckSrcContent(scopeConfiguration.Definitions, parentDir, out var scopeDefinition))
    {
        parentDir = parentDir.Skip(scopeDefinition!.PathRoot.Split('/').Length).ToArray();
        if (parentDir.Any())
        {
            var projectName = parentDir.First();
            if (projectName.StartsWith(scopeDefinition.ProjectPrefix))
            {
                projectName = projectName.Substring(scopeDefinition.ProjectPrefix.Length);
            }
            parentDir = new[] {projectName}.Concat(parentDir.Skip(1)).ToArray();
        }

        scope = $"{scopeDefinition.ScopePrefix}{string.Join('.', parentDir)}".ToLower();
        return true;
    }
    
    scope = string.Join('/', parentDir).ToLower();
    return true;
}

string[] GetParentPath(ScopeConfiguration configuration, IEnumerable<string> files)
{
    return files
        .Select(file => 
        {
            var redefinition = configuration.Redefinitions.SingleOrDefault(r => r.Filename == file);
            if (redefinition is not null)
            {
                return Path.Combine(redefinition.PathRoot, file);
            }

            return file;
        })
        .Select(Path.GetDirectoryName)
        .Select(file => file!.Replace(@"\", "/").Split('/'))
        .Aggregate((a, b) =>
        {
            if (CheckSrcContent(configuration.Definitions, a, out _) ^
                CheckSrcContent(configuration.Definitions, b, out _))
                throw new Exception(
                    "These files are not all 'src' or 'non-src', separate them with different commits");
            return a.Intersect(b).ToArray();
        });
}

bool CheckSrcContent(SrcScopeDefinition[] definitions, string[] path, out SrcScopeDefinition? scopeDefinition)
{
    foreach (var definition in definitions)
    {
        if (string.Join('/', path).StartsWith(definition.PathRoot))
        {
            scopeDefinition = definition;
            return true;
        }
    }
    
    scopeDefinition = null;
    return false;
}

ScopeConfiguration GetScopeConfiguration(string filename)
{
    var json = File.ReadAllText(filename, Encoding.UTF8);
    return JsonSerializer.Deserialize<ScopeConfiguration>(json)!;
}

internal class SrcScopeDefinition
{
    public string PathRoot { get; set; } = null!;
    public string ProjectPrefix { get; set; } = null!;
    public string ScopePrefix { get; set; } = null!;
}

internal class ScopeRedefinition
{
    public string Filename { get; set; } = null!;
    public string PathRoot { get; set; } = null!;
}

internal class ScopeConfiguration
{
    public SrcScopeDefinition[] Definitions { get; set; } = null!;
    public ScopeRedefinition[] Redefinitions { get; set; } = null!;
}