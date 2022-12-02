using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
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
        // 标记用的Attribute类
        var attributeSymbol =
            context.Compilation.GetTypeByMetadataName("DoliteTemplate.Api.Utils.RestApi.GenerateAPIAttribute");

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
                var attributes = classDeclaration.DescendantNodes().OfType<AttributeSyntax>();
                // 判断是否需要生成API Controller
                var generateApi = attributes.Any(attribute =>
                    semanticModel.GetTypeInfo(attribute).Type?.Name == attributeSymbol!.Name);
                if (!generateApi) continue;

                var serviceClass = semanticModel.GetDeclaredSymbol(classDeclaration);
                if (serviceClass is null) continue;
                var (filename, source) = GenerateController(serviceClass);
                context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    /// <summary>
    /// 生成API Controller
    /// </summary>
    /// <param name="class"></param>
    private static (string, string) GenerateController(INamedTypeSymbol @class)
    {
        var serviceName = @class.Name;
        var serviceFullName = @class.ToDisplayString();
        const string serviceSuffix = "Service";
        const string controllerSuffix = "Controller";
        var controllerName = serviceName.EndsWith(serviceSuffix)
            ? serviceName.Substring(0, serviceName.Length - serviceSuffix.Length)
            : serviceName;
        controllerName += controllerSuffix;
        var comments = GetComment(@class);
        var filename = $"{controllerName}.g.cs";


        var methods = GetAllMembers(@class);

        var methodSources = string.Join(Environment.NewLine,
            methods.Select(method => GenerateMethod(method, serviceName)));

        var source =
            $$"""
            namespace DoliteTemplate.Api.Controllers;

            {{comments}}
            [Microsoft.AspNetCore.Mvc.ApiController]
            [Microsoft.AspNetCore.Mvc.Route("[controller]")]
            public partial class {{controllerName}} : Microsoft.AspNetCore.Mvc.ControllerBase
            {
                private readonly {{serviceFullName}} _{{serviceName}}; 
                public {{controllerName}}({{serviceFullName}} {{serviceName}})
                {
                    _{{serviceName}} = {{serviceName}};
                }
                
                {{methodSources}}
            }
            """;

        return (filename, source);
    }

    private static string GenerateMethod(IMethodSymbol method, string serviceName)
    {
        
        var serviceReturnType = (method.ReturnType as INamedTypeSymbol)!;
        var isAsync = serviceReturnType.ContainingNamespace.ToDisplayString() == "System.Threading.Tasks" && serviceReturnType.MetadataName.StartsWith("Task");
        var returnType = isAsync ? "System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult>" : "Microsoft.AspNetCore.Mvc.ActionResult";
        var hasResult = isAsync ? serviceReturnType.TypeArguments.Any() : serviceReturnType.ToDisplayString() != "void";
        var resultType = hasResult ? isAsync ? serviceReturnType.TypeArguments.First().ToDisplayString() : serviceReturnType.ToDisplayString() : string.Empty;
        var returnTypeSource = string.IsNullOrEmpty(resultType) ? string.Empty : $"typeof({resultType}), ";
        var attributes = string.Join(Environment.NewLine, method.GetAttributes().Select(attribute => $"[{attribute}]"));
        var parameters = method.Parameters.Select(GenerateParameter).ToArray();
        var parameterDefinitionSources = string.Join(", ", parameters.Select(tuple => tuple.definition));
        var parameterUsageSources = string.Join(", ", parameters.Select(tuple => tuple.usage));
        var comments = GetComment(method);
        var source =
            $$"""
            {{comments}}
            {{attributes}}
            [Microsoft.AspNetCore.Mvc.ProducesResponseType({{returnTypeSource}}Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
            [Microsoft.AspNetCore.Mvc.ProducesResponseType(typeof(DoliteTemplate.Api.Utils.Error.ErrorInfo), Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
            {{method.DeclaredAccessibility.ToString().ToLower()}} {{(isAsync ? "async " : string.Empty)}}{{returnType}} {{method.Name}}(
                {{parameterDefinitionSources}})
            {
                {{(hasResult ? "var result = " : string.Empty)}}{{(isAsync ? "await " : string.Empty)}}_{{serviceName}}.{{method.Name}}(
                    {{parameterUsageSources}});

                return Ok({{(hasResult ? "result" : string.Empty)}});
            }
            """;

        return source;
    }

    private static (string definition, string usage) GenerateParameter(IParameterSymbol parameter)
    {
        var type = parameter.Type.ToDisplayString();
        var name = parameter.Name;
        var definition = $"{type} {name}";
        var attributes = string.Join(Environment.NewLine, parameter.GetAttributes().Select(attribute => $"[{attribute}]"));
        if (!string.IsNullOrEmpty(attributes)) definition = $"{attributes} {definition}";

        return (definition, name);
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
            if (baseType is not null && @class.ToDisplayString() != "DoliteTemplate.Api.Services.Base.BaseService")
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
            attribute.AttributeClass!.ContainingNamespace.ToDisplayString() == "Microsoft.AspNetCore.Mvc" &&
            attribute.AttributeClass!.Name.StartsWith("Http"));
    }

    private static string GetComment(ISymbol symbol)
    {
        var memberXml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(memberXml)) return string.Empty;
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(memberXml);
        var comments = xmlDoc.FirstChild.InnerXml;

        return string.Join(Environment.NewLine,
            comments.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(comment => $"/// {comment.Trim()}"));
    }
}