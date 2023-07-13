using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace DoliteTemplate.CodeGenerator;

[Generator]
public class ControllerGenerator : ISourceGenerator
{
    private static readonly IEnumerable<string> ControllerIgnoreAttributes = new[]
    {
        Symbols.Types.Project.ApiServiceAttribute
    };

    private static readonly Dictionary<string, string> ControllerDefaultAttributes = new()
    {
        {
            Symbols.Types.System.ApiControllerAttribute,
            Symbols.Types.BuildAttribute(Symbols.Types.System.ApiControllerAttribute)
        },
        {
            Symbols.Types.System.RouteAttribute,
            Symbols.Types.BuildAttribute(Symbols.Types.System.RouteAttribute, "\"[controller]\"")
        }
    };

    private static readonly IEnumerable<string> MethodIgnoreAttributes = new[]
    {
        Symbols.Types.Project.TransactionAttribute
    };

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // 筛选有Attribute的类的树
        var trees = context.Compilation.SyntaxTrees
            .Where(st => st.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()));

        // 每棵树代表一个.cs文件
        foreach (var tree in trees)
        {
            var semanticModel = context.Compilation.GetSemanticModel(tree);
            // 遍历树中的每个类
            foreach (var classDeclaration in tree
                         .GetRoot()
                         .DescendantNodes()
                         .OfType<ClassDeclarationSyntax>()
                         .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any()))
            {
                var @class = semanticModel.GetDeclaredSymbol(classDeclaration);
                if (@class is null) continue;

                if (!@class.GetAttributes().Any(attribute =>
                        attribute.AttributeClass!.ToDisplayString() == Symbols.Types.Project.ApiServiceAttribute))
                    continue;

                var sourceBuilder = new StringBuilder();
                var filename = GenerateController(sourceBuilder, @class);
                var source = sourceBuilder.ToString();
                context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    private static string GenerateController(StringBuilder builder, ITypeSymbol @class)
    {
        var serviceName = @class.Name;
        var serviceFullName = @class.ToDisplayString();
        var apiServiceAttribute = @class.GetAttributes().Single(attribute =>
            attribute.AttributeClass!.ToDisplayString() == Symbols.Types.Project.ApiServiceAttribute);
        string? tag = null;
        var rule = string.Empty;
        foreach (var argPair in apiServiceAttribute.NamedArguments)
            switch (argPair.Key)
            {
                case "Tag":
                    tag = (string?)argPair.Value.Value;
                    break;
                case "Rule":
                    rule = (string)argPair.Value.Value!;
                    break;
            }

        var entityName = tag ?? GetNameExcludingSuffix(serviceName, Symbols.Suffixes.Service);
        var controllerName = entityName + Symbols.Suffixes.Controller;
        var filename = $"{controllerName}.g.cs";

        // nullable macro
        builder.AppendLine("#nullable enable").AppendLine();

        // Namespace
        builder.AppendLine($"namespace {Symbols.Namespaces.Project.Controllers};");
        builder.AppendLine();

        // Class comments
        GenerateComments(builder, @class, 0);

        // Class attributes
        var attributes = @class.GetAttributes().Where(attribute =>
            !ControllerIgnoreAttributes.Contains(attribute.AttributeClass!.ToDisplayString())).ToArray();
        var attributeLines = attributes.Select(Symbols.Types.BuildAttribute);
        foreach (var attribute in attributeLines)
        {
            builder.AppendLine(attribute);
        }

        var defaultAttributeLines = ControllerDefaultAttributes.Keys
            .Except(attributes.Select(attribute => attribute.AttributeClass!.ToDisplayString()))
            .Select(key => ControllerDefaultAttributes[key]);
        foreach (var attribute in defaultAttributeLines)
        {
            builder.AppendLine(attribute);
        }

        // Class header
        builder.AppendLine($"public partial class {controllerName} : {Symbols.Types.System.ControllerBase}");
        builder.AppendLine("{");

        // Service member
        var serviceMemberName = serviceName;
        builder.Append(Symbols.Codes.Ident)
            .AppendLine($"public {serviceFullName} {serviceMemberName} {{ get; init; }} = null!;")
            .AppendLine();

        // Methods
        if (string.IsNullOrEmpty(rule)) rule = ".*";

        var methods = GetHttpMethods(@class, rule);
        foreach (var method in methods)
        {
            GenerateMethod(builder, method, serviceMemberName);
            builder.AppendLine();
        }

        builder.AppendLine("}");

        return filename;
    }

    private static void GenerateMethod(StringBuilder builder, IMethodSymbol method, string serviceMemberName)
    {
        var serviceReturnType = (method.ReturnType as INamedTypeSymbol)!;
        var isAsync = serviceReturnType.ContainingNamespace.ToDisplayString() == Symbols.Namespaces.System.Tasks &&
                      serviceReturnType.MetadataName.StartsWith("Task");
        var hasResult = isAsync ? serviceReturnType.TypeArguments.Any() : serviceReturnType.ToDisplayString() != "void";
        var resultType = string.Empty;
        if (hasResult)
        {
            if (isAsync)
            {
                var arg = (INamedTypeSymbol)serviceReturnType.TypeArguments.Single();
                resultType = TrimNullable(arg).ToDisplayString();
            }
            else
            {
                resultType = serviceReturnType.ToDisplayString();
            }
        }

        // Method comments
        GenerateComments(builder, method, 1);

        // Method attributes
        var attributes = GetAttributesIncludeOverride(method);
        var isTransaction = IsTransaction(method);
        var attributeLines = attributes.Select(Symbols.Types.BuildAttribute);
        foreach (var attribute in attributeLines)
        {
            builder.Append(Symbols.Codes.Ident).AppendLine(attribute);
        }

        // Response types
        var okResponseAttribute = string.IsNullOrEmpty(resultType)
            ? Symbols.Types.BuildAttribute(Symbols.Types.System.ProducesResponseTypeAttribute,
                $"{Symbols.Types.System.StatusCodes}.Status200OK")
            : Symbols.Types.BuildAttribute(Symbols.Types.System.ProducesResponseTypeAttribute,
                Symbols.Types.BuildTypeOf(resultType), $"{Symbols.Types.System.StatusCodes}.Status200OK");
        var badRequestResponseAttribute = Symbols.Types.BuildAttribute(
            Symbols.Types.System.ProducesResponseTypeAttribute,
            Symbols.Types.BuildTypeOf(Symbols.Types.Project.ErrorInfo),
            $"{Symbols.Types.System.StatusCodes}.Status400BadRequest");
        foreach (var attribute in new[] { okResponseAttribute, badRequestResponseAttribute })
        {
            builder.Append(Symbols.Codes.Ident).AppendLine(attribute);
        }

        // Accessibility
        builder.Append(Symbols.Codes.Ident)
            .AppendFormat("{0} ", method.DeclaredAccessibility.ToString().ToLower());

        // Is async
        if (isAsync)
            builder.AppendFormat("async {0}<{1}> ", Symbols.Types.System.Task,
                Symbols.Types.System.ActionResult);
        else
            builder.AppendFormat("{0} ", Symbols.Types.System.ActionResult);

        // Method name
        builder.Append(method.Name);

        // Parameter definitions
        builder.Append("(");
        var lastParameter = method.Parameters.LastOrDefault();
        if (lastParameter is not null) builder.AppendLine();

        foreach (var parameter in method.Parameters)
        {
            GenerateParameterDefinition(builder, parameter);
            if (!ReferenceEquals(lastParameter, parameter)) builder.AppendLine(",");
        }

        builder.AppendLine(")");

        // Method body
        builder.Append(Symbols.Codes.Ident).AppendLine("{");

        // Start transaction
        if (isTransaction)
        {
            builder.Append(Symbols.Codes.Ident).Append(Symbols.Codes.Ident).AppendFormat(
                isAsync
                    ? "await using var transaction = await {0}.DbContext.Database.BeginTransactionAsync();"
                    : "using var transaction = {0}.DbContext.Database.BeginTransaction();",
                serviceMemberName).AppendLine();
        }

        builder.Append(Symbols.Codes.Ident).Append(Symbols.Codes.Ident);
        if (hasResult) builder.Append("var result = ");

        if (isAsync) builder.Append("await ");

        builder.AppendFormat("{0}.{1}", serviceMemberName, method.Name);

        // Parameter usages
        builder.Append("(");
        lastParameter = method.Parameters.LastOrDefault();
        if (lastParameter is not null) builder.AppendLine();

        foreach (var parameter in method.Parameters)
        {
            GenerateParameterUsage(builder, parameter);
            if (!ReferenceEquals(lastParameter, parameter)) builder.AppendLine(",");
        }

        builder.AppendLine(");");

        // Commit transaction
        if (isTransaction)
        {
            builder.Append(Symbols.Codes.Ident).Append(Symbols.Codes.Ident).AppendFormat(
                isAsync
                    ? "await transaction.CommitAsync();"
                    : "transaction.Commit();",
                serviceMemberName).AppendLine();
        }

        // Return
        builder.AppendLine().Append(Symbols.Codes.Ident).Append(Symbols.Codes.Ident).Append("return Ok(");
        if (hasResult) builder.Append("result");

        builder.AppendLine(");");

        builder.Append(Symbols.Codes.Ident).AppendLine("}");
    }

    private static INamedTypeSymbol TrimNullable(INamedTypeSymbol type)
    {
        if (type.NullableAnnotation != NullableAnnotation.Annotated)
        {
            return type;
        }

        return (INamedTypeSymbol)(type.TypeArguments.SingleOrDefault() ?? type.OriginalDefinition);
    }

    private static void GenerateParameterDefinition(StringBuilder builder, IParameterSymbol parameter)
    {
        foreach (var attribute in parameter.GetAttributes())
            builder.Append(Symbols.Codes.Ident)
                .Append(Symbols.Codes.Ident)
                .AppendLine($"[{attribute}]");

        builder.Append(Symbols.Codes.Ident).Append(Symbols.Codes.Ident);
        if (parameter.HasExplicitDefaultValue)
        {
            var @default = GetCodeDisplayValue(parameter.ExplicitDefaultValue);
            builder.AppendFormat("{0} {1} = {2}", parameter.Type.ToDisplayString(), parameter.Name, @default);
        }
        else
        {
            builder.AppendFormat("{0} {1}", parameter.Type.ToDisplayString(), parameter.Name);
        }
    }

    private static string GetCodeDisplayValue(object? value)
    {
        return value switch
        {
            bool @bool => @bool.ToString().ToLower(),
            char @char => $"'{@char}'",
            string @string => $"\"{@string}\"",
            _ => string.IsNullOrEmpty(value?.ToString()) ? "null" : value!.ToString()
        };
    }

    private static void GenerateParameterUsage(StringBuilder builder, IParameterSymbol parameter)
    {
        builder.Append(Symbols.Codes.Ident)
            .Append(Symbols.Codes.Ident)
            .Append(Symbols.Codes.Ident)
            .Append(parameter.Name);
    }

    private static void GenerateComments(StringBuilder builder, ISymbol symbol, int indentLevel)
    {
        var memberXml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(memberXml)) return;

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(memberXml);
        var nodes = xmlDoc.FirstChild.ChildNodes;
        foreach (XmlNode node in nodes)
        {
            var nodeLines = node.OuterXml.Split(new[]
            {
#if _WINDOWS
                "\r\n"
#else
                "\n"
#endif
            }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var nodeLine in nodeLines)
            {
                foreach (var _ in Enumerable.Range(0, indentLevel))
                {
                    builder.Append(Symbols.Codes.Ident);
                }

                builder.AppendFormat("/// {0}", nodeLine.Trim()).AppendLine();
            }
        }
    }

    private static string GetNameExcludingSuffix(string fullName, string suffix)
    {
        return fullName.EndsWith(suffix)
            ? fullName.Substring(0, fullName.Length - suffix.Length)
            : fullName;
    }

    private static IEnumerable<IMethodSymbol> GetHttpMethods(ITypeSymbol @class, string rule)
    {
        var regex = new Regex(rule, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        var ignoredMethods = new List<IMethodSymbol>();
        var hiddenMethodNames = new List<string>();
        while (true)
        {
            foreach (var method in @class.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.IsOverride && method.OverriddenMethod is not null)
                {
                    ignoredMethods.Add(method.OverriddenMethod);
                }

                if (hiddenMethodNames.Contains(method.Name))
                {
                    ignoredMethods.Add(method);
                }
                else
                {
                    hiddenMethodNames.Add(method.Name);
                }

                if (!method.IsStatic &&
                    method.Name != ".ctor" &&
                    method is { DeclaredAccessibility: Accessibility.Public, MethodKind: MethodKind.Ordinary } &&
                    !ignoredMethods.Contains(method) &&
                    HasHttpMethod(method) &&
                    regex.IsMatch(method.Name))
                    yield return method;
            }

            var baseType = @class.BaseType;
            if (baseType is not null && @class.ToDisplayString() != Symbols.Types.Project.BaseService)
            {
                @class = baseType;
                continue;
            }

            break;
        }
    }

    private static bool HasHttpMethod(IMethodSymbol method)
    {
        return method.GetAttributes().Any(attribute =>
                   attribute.AttributeClass!.ContainingNamespace.ToDisplayString() == Symbols.Namespaces.System.Mvc &&
                   attribute.AttributeClass!.Name.StartsWith("Http"))
               || (method.IsOverride && HasHttpMethod(method.OverriddenMethod!));
    }

    private static bool IsTransaction(IMethodSymbol method)
    {
        return GetAttributesIncludeOverride(method).Any(attribute =>
            string.Equals(Symbols.Types.Project.TransactionAttribute, attribute.AttributeClass!.ToDisplayString()));
    }

    private static AttributeData[] GetAttributesIncludeOverride(IMethodSymbol method)
    {
        var attribute = method.GetAttributes();
        return (!method.IsOverride
            ? attribute
            : attribute.Concat(GetAttributesIncludeOverride(method.OverriddenMethod!))).ToArray();
    }
}