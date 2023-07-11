namespace DoliteTemplate.CodeGenerator.Symbols;

public static class Namespaces
{
    public static class System
    {
        public const string Tasks = "System.Threading.Tasks";
        public const string Mvc = "Microsoft.AspNetCore.Mvc";
        public const string Http = "Microsoft.AspNetCore.Http";
        public const string GraphAttributes = "GraphQL.AspNet.Attributes";
    }

    public static class Project
    {
        public const string Api = "DoliteTemplate.Api";
        public const string Controllers = $"{Api}.Controllers";
        public const string Restful = $"{Controllers}.Restful";
        public const string Graph = $"{Controllers}.Graph";
    }
}