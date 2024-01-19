using System.Text;
using Microsoft.CodeAnalysis;

namespace DoliteTemplate.CodeGenerator;

public static class Symbols
{
    public static class Namespaces
    {
        public static class System
        {
            public const string Tasks = "System.Threading.Tasks";
            public const string Mvc = "Microsoft.AspNetCore.Mvc";
            public const string Http = "Microsoft.AspNetCore.Http";
            public const string EfCore = "Microsoft.EntityFrameworkCore";
            public const string Collections = "System.Collections.Generic";
        }

        public static class Project
        {
            public const string ApiShared = "DoliteTemplate.Api.Shared";
            public const string DomainShared = "DoliteTemplate.Domain.Shared";
        }
    }

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
            public const string Enumerable = $"{Namespaces.System.Collections}.IEnumerable";
            public const string Task = $"{Namespaces.System.Tasks}.Task";
            public const string ControllerBase = $"{Namespaces.System.Mvc}.ControllerBase";
            public const string ActionResult = $"{Namespaces.System.Mvc}.ActionResult";
            public const string StatusCodes = $"{Namespaces.System.Http}.StatusCodes";
            public const string ApiControllerAttribute = $"{Namespaces.System.Mvc}.ApiControllerAttribute";
            public const string HttpGetAttribute = $"{Namespaces.System.Mvc}.HttpGetAttribute";
            public const string RouteAttribute = $"{Namespaces.System.Mvc}.RouteAttribute";

            public const string ProducesResponseTypeAttribute =
                $"{Namespaces.System.Mvc}.ProducesResponseTypeAttribute";

            public const string AsyncStateMachineAttribute =
                "System.Runtime.CompilerServices.AsyncStateMachineAttribute";

            public const string DebuggerStepThroughAttribute = "System.Diagnostics.DebuggerStepThroughAttribute";
            public const string NullableAttribute = "System.Runtime.CompilerServices.NullableAttribute";
        }

        public static class Project
        {
            public const string ApiServiceAttribute = $"{Namespaces.Project.ApiShared}.Utils.ApiServiceAttribute";
            public const string ApiCommentAttribute = $"{Namespaces.Project.ApiShared}.Utils.ApiCommentAttribute";
            public const string BaseService = $"{Namespaces.Project.ApiShared}.Services.BaseService";
            public const string EntityCrudService = $"{Namespaces.Project.ApiShared}.Services.EntityCrudService";
            public const string ErrorInfo = $"{Namespaces.Project.ApiShared}.Errors.ErrorInfo";
            public const string TransactionAttribute = $"{Namespaces.Project.ApiShared}.Utils.TransactionAttribute";

            public const string QueryParameterAttribute =
                $"{Namespaces.Project.ApiShared}.Utils.QueryParameterAttribute";

            public const string PaginatedList = $"{Namespaces.Project.DomainShared}.Utils.PaginatedList";
        }
    }

    public static class Suffixes
    {
        public const string Controller = "Controller";
        public const string Service = "Service";
    }

    public static class Codes
    {
        public const string Ident = "    ";
        public const string NewLine = "\n";
    }
}