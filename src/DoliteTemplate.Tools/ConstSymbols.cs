namespace DoliteTemplate.Tools;

public static class ConstSymbols
{
    public static class Namespaces
    {
        public static class System
        {
            public const string Tasks = "System.Threading.Tasks";
            public const string Mvc = "Microsoft.AspNetCore.Mvc";
            public const string Http = "Microsoft.AspNetCore.Http";
        }

        public static class Project
        {
            public const string Api = "DoliteTemplate.Api";
            public const string Controllers = $"{Api}.Controllers";
        }
    }

    public static class Types
    {
        public static class System
        {
            public const string Task = $"{Namespaces.System.Tasks}.Task";
            public const string ControllerBase = $"{Namespaces.System.Mvc}.ControllerBase";
            public const string ActionResult = $"{Namespaces.System.Mvc}.ActionResult";
            public const string StatusCodes = $"{Namespaces.System.Http}.StatusCodes";
            public const string ApiControllerAttribute = $"{Namespaces.System.Mvc}.ApiControllerAttribute";
            public const string RouteAttribute = $"{Namespaces.System.Mvc}.RouteAttribute";
            public const string ProducesResponseTypeAttribute = $"{Namespaces.System.Mvc}.ProducesResponseTypeAttribute";
        }

        public static class Project
        {
            public const string BaseService = $"{Namespaces.Project.Api}.Services.Base.BaseService";
            public const string ErrorInfo = $"{Namespaces.Project.Api}.Utils.Error.ErrorInfo";
            public const string ApiServiceAttribute = $"{Namespaces.Project.Api}.Utils.ApiServiceAttribute";
        }
    }

    public static class Suffixes
    {
        public const string Controller = "Controller";
        public const string Service = "Service";
    }
}