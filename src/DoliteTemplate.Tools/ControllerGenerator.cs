using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace DoliteTemplate.Tools;

[Generator]
public class ControllerGenerator : ISourceGenerator
{
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
                        attribute.AttributeClass!.ToDisplayString() == ConstSymbols.Types.Project.ApiServiceAttribute))
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
        var entityName =
            (string?)@class.GetAttributes()
                .Single(attribute => attribute.AttributeClass!.ToDisplayString() ==
                                     ConstSymbols.Types.Project.ApiServiceAttribute).ConstructorArguments
                .FirstOrDefault().Value ?? GetNameExcludingSuffix(serviceName, ConstSymbols.Suffixes.Service);
        var controllerName = entityName + ConstSymbols.Suffixes.Controller;
        var filename = $"{controllerName}.g.cs";

        // Namespace
        builder.AppendLine($"namespace {ConstSymbols.Namespaces.Project.Controllers};");
        builder.AppendLine();

        // Class comments
        GenerateComments(builder, @class, 0);

        // ApiController
        builder.AppendLine($"[{ConstSymbols.Types.System.ApiControllerAttribute}]");

        // Route
        builder.AppendLine($"[{ConstSymbols.Types.System.RouteAttribute}(\"[controller]\")]");

        // Class header
        builder.AppendLine($"public partial class {controllerName} : {ConstSymbols.Types.System.ControllerBase}");
        builder.AppendLine("{");

        // Service member
        var serviceMemberName = $"_{serviceName}";
        builder.Append(ConstSymbols.Codes.Ident)
            .AppendLine($"private readonly {serviceFullName} {serviceMemberName};")
            .AppendLine();

        // Constructor
        builder.Append(ConstSymbols.Codes.Ident)
            .AppendLine($"public {controllerName}({serviceFullName} {serviceName})");
        builder.Append(ConstSymbols.Codes.Ident).AppendLine("{");
        builder.Append(ConstSymbols.Codes.Ident).Append(ConstSymbols.Codes.Ident)
            .AppendLine($"_{serviceName} = {serviceName};");
        builder.Append(ConstSymbols.Codes.Ident).AppendLine("}").AppendLine();

        // Methods
        var methods = GetAllMembers(@class);
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
        var isAsync = serviceReturnType.ContainingNamespace.ToDisplayString() == ConstSymbols.Namespaces.System.Tasks &&
                      serviceReturnType.MetadataName.StartsWith("Task");
        var hasResult = isAsync ? serviceReturnType.TypeArguments.Any() : serviceReturnType.ToDisplayString() != "void";
        var resultType = hasResult
            ? isAsync ? serviceReturnType.TypeArguments.First().ToDisplayString() : serviceReturnType.ToDisplayString()
            : string.Empty;

        // Method comments
        GenerateComments(builder, method, 1);

        // Method attributes
        foreach (var attribute in method.GetAttributes())
            builder.Append(ConstSymbols.Codes.Ident).AppendLine($"[{attribute}]");

        // 200 OK
        builder.Append(ConstSymbols.Codes.Ident)
            .Append("[")
            .Append(ConstSymbols.Types.System.ProducesResponseTypeAttribute)
            .Append($"(typeof({resultType}), {ConstSymbols.Types.System.StatusCodes}.Status200OK)")
            .Append("]")
            .AppendLine();

        // 400 Bad Request
        builder.Append(ConstSymbols.Codes.Ident)
            .Append("[")
            .Append(ConstSymbols.Types.System.ProducesResponseTypeAttribute)
            .Append(
                $"(typeof({ConstSymbols.Types.Project.ErrorInfo}), {ConstSymbols.Types.System.StatusCodes}.Status400BadRequest)")
            .Append("]")
            .AppendLine();

        // Accessibility
        builder.Append(ConstSymbols.Codes.Ident)
            .AppendFormat("{0} ", method.DeclaredAccessibility.ToString().ToLower());

        // Is async
        if (isAsync)
            builder.AppendFormat("async {0}<{1}> ", ConstSymbols.Types.System.Task,
                ConstSymbols.Types.System.ActionResult);
        else
            builder.AppendFormat("{0} ", ConstSymbols.Types.System.ActionResult);
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
        builder.Append(ConstSymbols.Codes.Ident).AppendLine("{");
        builder.Append(ConstSymbols.Codes.Ident).Append(ConstSymbols.Codes.Ident);
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

        builder.AppendLine(");").AppendLine();

        // Return
        builder.Append(ConstSymbols.Codes.Ident).Append(ConstSymbols.Codes.Ident).Append("return Ok(");
        if (hasResult) builder.Append("result");
        builder.AppendLine(");");

        builder.Append(ConstSymbols.Codes.Ident).AppendLine("}");
    }

    private static void GenerateParameterDefinition(StringBuilder builder, IParameterSymbol parameter)
    {
        foreach (var attribute in parameter.GetAttributes())
            builder.Append(ConstSymbols.Codes.Ident)
                .Append(ConstSymbols.Codes.Ident)
                .AppendLine($"[{attribute}]");
        builder.Append(ConstSymbols.Codes.Ident)
            .Append(ConstSymbols.Codes.Ident)
            .AppendFormat("{0} {1}", parameter.Type.ToDisplayString(), parameter.Name);
    }

    private static void GenerateParameterUsage(StringBuilder builder, IParameterSymbol parameter)
    {
        builder.Append(ConstSymbols.Codes.Ident)
            .Append(ConstSymbols.Codes.Ident)
            .Append(ConstSymbols.Codes.Ident)
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
            var nodeLines = node.OuterXml.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var nodeLine in nodeLines)
            {
                foreach (var _ in Enumerable.Range(0, indentLevel))
                    builder.Append(ConstSymbols.Codes.Ident);
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

    private static IEnumerable<IMethodSymbol> GetAllMembers(ITypeSymbol @class)
    {
        while (true)
        {
            foreach (var method in @class.GetMembers().OfType<IMethodSymbol>())
                if (!method.IsStatic &&
                    method.Name != ".ctor" &&
                    method.DeclaredAccessibility == Accessibility.Public &&
                    method.MethodKind == MethodKind.Ordinary &&
                    HasHttpMethod(method))
                    yield return method;
            var baseType = @class.BaseType;
            if (baseType is not null && @class.ToDisplayString() != ConstSymbols.Types.Project.BaseService)
            {
                @class = baseType;
                continue;
            }

            break;
        }
    }

    private static bool HasHttpMethod(ISymbol method)
    {
        return method.GetAttributes().Any(attribute =>
            attribute.AttributeClass!.ContainingNamespace.ToDisplayString() == ConstSymbols.Namespaces.System.Mvc &&
            attribute.AttributeClass!.Name.StartsWith("Http"));
    }
}