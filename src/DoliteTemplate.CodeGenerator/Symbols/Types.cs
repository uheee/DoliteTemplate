using System.Text;
using Microsoft.CodeAnalysis;

namespace DoliteTemplate.CodeGenerator.Symbols;

public static class Types
{
    public static string BuildAttribute(AttributeData attributeData)
    {
        return $"[{attributeData}]";
    }

    public static string BuildAttribute(string typename, params string[] @params)
    {
        var builder = new StringBuilder("[$attr($params)]");
        builder.Replace("$attr", typename);
        var parameters = string.Join(", ", @params);
        builder.Replace("$params", parameters);
        return builder.ToString();
    }

    public static string BuildTypeOf(string typename)
    {
        return $"typeof({typename})";
    }

    public static class System
    {
        public const string Task = $"{Namespaces.System.Tasks}.Task";
        public const string ControllerBase = $"{Namespaces.System.Mvc}.ControllerBase";
        public const string GraphController = "GraphQL.AspNet.Controllers.GraphController";
        public const string ActionResult = $"{Namespaces.System.Mvc}.ActionResult";
        public const string GraphResult = $"GraphQL.AspNet.Interfaces.Controllers.IGraphActionResult";
        public const string StatusCodes = $"{Namespaces.System.Http}.StatusCodes";
        public const string ApiControllerAttribute = $"{Namespaces.System.Mvc}.ApiControllerAttribute";
        public const string RouteAttribute = $"{Namespaces.System.Mvc}.RouteAttribute";

        public const string ProducesResponseTypeAttribute =
            $"{Namespaces.System.Mvc}.ProducesResponseTypeAttribute";

        public const string PossibleTypesAttribute =
            $"GraphQL.AspNet.Attributes.PossibleTypesAttribute";
    }

    public static class Project
    {
        public const string BaseService = $"{Namespaces.Project.Api}.Services.Base.BaseService";
        public const string ErrorInfo = $"{Namespaces.Project.Api}.Utils.Error.ErrorInfo";
        public const string ApiServiceAttribute = $"{Namespaces.Project.Api}.Utils.ApiServiceAttribute";
        public const string TransactionAttribute = $"{Namespaces.Project.Api}.Utils.TransactionAttribute";
    }
}