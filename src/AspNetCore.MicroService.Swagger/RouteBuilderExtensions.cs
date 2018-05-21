using System.Collections.Generic;
using System.Linq;
using AspNetCore.MicroService.Routing.Abstractions;
using AspNetCore.MicroService.Routing.Abstractions.Builder;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCore.MicroService.Swagger
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder UseSwagger(this IRouteBuilder routeBuilder)
        {
            var t = routeBuilder.AllRoutes
                .SelectMany(r => r.Metadatas).ToList();
            
            IDictionary<string, PathItem> pathItems = routeBuilder.AllRoutes
                .SelectMany(r => r.Metadatas)
                .GroupBy(m => m.RelativePath)
                .ToDictionary(m => "/" + m.First().RelativePath, m =>
                    new PathItem
                    {
                        Get = CreateOperation(m, HttpMethods.Get),
                        Post = CreateOperation(m, HttpMethods.Post),
                        Put = CreateOperation(m, HttpMethods.Put),
                        Delete = CreateOperation(m, HttpMethods.Delete)
                        
                    });
            MicroServiceSwaggerGenerator.AddPaths(pathItems);
            return routeBuilder;
        }

        private static Operation CreateOperation(IEnumerable<RouteActionMetadata> metadatas, string httpMethod)
        {
            RouteActionMetadata metadata = metadatas.FirstOrDefault(m => m.HttpMethod == httpMethod);
            if (metadata == null) return null;

            string operationId = metadata.RelativePath.Capitalize();
            var operation = new Operation
            {
                OperationId = operationId,
                Tags = new List<string> {operationId},
                Responses = SuccessResponses()
            };
            return operation;
        }

        private static Dictionary<string, Response> SuccessResponses()
        {
            return new Dictionary<string, Response>
            {
                {
                    "200",
                    new Response
                    {
                        Description = "Success"
                    }
                }
            };
        }
    }
}