#nullable enable

#load "scope-calc.csx"

using System.Text.RegularExpressions;

var filename = Args.First();
var messageLines = File.ReadAllLines(filename);
if (!CommitMessage.TryParse(messageLines, out var commitMessage) || commitMessage is null) return 1;

// auto set scope
if (string.IsNullOrEmpty(commitMessage.Title.Scope) && GetScope(Args.Skip(1), out var scope)) commitMessage.Title.Scope = scope;
// attach breaking flag
commitMessage.Title.Breaking = commitMessage.Footers?.Any(footer => footer.Key == "BREAKING CHANGE") ?? false;
// check revert refs
if (commitMessage.Title.Type == "revert" && !(commitMessage.Footers?.Any(footer => footer.Key == "Refs") ?? false))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Revert commit message requires a 'Refs' footer");
    Console.ResetColor();

    return 1;
}

PrintDebug(commitMessage);
WriteBackMessage(filename, commitMessage);
return 0;

void PrintDebug(CommitMessage message)
{
    Console.WriteLine($$"""
Title: {
    type: {{message!.Title.Type}},
    scope: {{(string.IsNullOrEmpty(message.Title.Scope) ? "NULL" : message.Title.Scope)}},
    breaking: {{message.Title.Breaking}},
    subject: {{message.Title.Subject}}
}
""");
    
    if (message.Body is null || !message.Body.Any()) Console.WriteLine("Body: NULL");
    else
    {
        Console.WriteLine("Body:");
        foreach (var content in message.Body)
        {
            Console.WriteLine(content);
        }
    }
    
    if (message.Footers is null || !message.Footers.Any()) Console.WriteLine("Footers: NULL");
    else
    {
        Console.WriteLine("Footers:");
        foreach (var content in message.Footers)
        {
            Console.WriteLine($"<{content.Key}>: <{content.Value}>");
        }
    }
}

void WriteBackMessage(string file, CommitMessage message)
{
    var text = new StringBuilder();
    // title
    text.Append(message.Title.Type);
    if (!string.IsNullOrEmpty(message.Title.Scope))
        text.Append($"({message.Title.Scope})");
    if (message.Title.Breaking)
        text.Append('!');
    text.Append(": ");
    text.Append(message.Title.Subject);
    
    // body
    if (message.Body is not null)
    {
        text.AppendLine().AppendLine();
        foreach (var body in message.Body)
            text.AppendLine(body);
    }
    
    // footers
    if (message.Footers is not null)
    {
        text.AppendLine();
        foreach (var footer in message.Footers)
            text.AppendLine($"{footer.Key}: {footer.Value}");
    }
    
    File.WriteAllText(file, text.ToString());
}

internal class CommitMessage
{
    private static readonly Regex FooterRegex =
        new(@"^(?<key>([A-Z0-9]+[a-z0-9]*(\-[a-z0-9]+)*)|(BREAKING CHANGE))((\:\s)|(\s\#))(?<value>.+)$");
    
    public CommitTitle Title { get; private set; } = null!;
    public string[]? Body { get; private set; }
    public KeyValuePair<string, string>[]? Footers { get; private set; }

    public static bool TryParse(string[] lines, out CommitMessage? message)
    {
        message = null;
        if (!CommitTitle.TryParse(lines.First(), out var title)) return false;
        message = new CommitMessage {Title = title!};
        if (lines.Length <= 1) return true;
        var blocks = new List<List<string>>();
        var blockLines = new List<string>();
        foreach (var line in lines.Skip(1))
            if (string.IsNullOrEmpty(line))
            {
                blockLines = new List<string>();
            }
            else
            {
                if (!blocks.Contains(blockLines)) blocks.Add(blockLines);
                blockLines.Add(line);
            }
        if (!blocks.Any()) return true;

        var footersInfo = blocks.Select((block, i) =>
        {
            var result = CheckFooters(block, out var footers);
            return new {i, result, footers};
        }).ToArray();

        var footerIndex = footersInfo.Reverse().FirstOrDefault(info => !info.result)?.i ?? -1;
        var footers = footersInfo[(footerIndex + 1)..]
            .SelectMany(info => info.footers)
            .ToArray();

        message.Body = GetBody(blocks.ToArray()[..(footerIndex + 1)]);
        message.Footers = footers;
        
        return true;
    }

    private static bool CheckFooters(IEnumerable<string> lines, out List<KeyValuePair<string, string>> footers)
    {
        footers = new List<KeyValuePair<string, string>>();
        foreach (var line in lines)
        {
            var match = FooterRegex.Match(line);
            if (!match.Success)
            {
                footers.Clear();
                return false;
            }
            var key = match.Groups["key"].Value;
            var value = match.Groups["value"].Value;
            footers.Add(new KeyValuePair<string, string>(key, value));
        }
        
        return true;
    }
    
    private static string[] GetBody(IEnumerable<IEnumerable<string>> blocks)
    {
        return blocks.Select(block => string.Join(' ', block)).ToArray();
    }
}

internal class CommitTitle
{
    public string Type { get; set; } = null!;
    public string? Scope { get; set; }
    public bool Breaking { get; set; }
    public string? Subject { get; set; }
    
    private static readonly Regex TitleRegex =
        new(@"^(?<type>build|chore|ci|docs|feat|fix|perf|refactor|revert|style|test)(\((?<scope>.+)\))?(?<breaking>!)?:\s(?<subject>.*)$");

    public static bool TryParse(string message, out CommitTitle? title)
    {
        title = null;
        var match = TitleRegex.Match(message);
        if (!match.Success)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid commit message title: '{message}'");
            Console.ResetColor();
            Console.WriteLine("e.g: 'feat(scope): subject' or 'fix: subject'");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("more info: https://www.conventionalcommits.org/en/v1.0.0/");

            return false;
        }

        title = new CommitTitle
        {
            Type = match.Groups["type"].Value,
            Scope = match.Groups["scope"].Value,
            Breaking = match.Groups["breaking"].Success,
            Subject = match.Groups["subject"].Value
        };

        return true;
    }
}