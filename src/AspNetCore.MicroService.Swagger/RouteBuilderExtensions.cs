using AspNetCore.MicroService.Routing.Abstractions.Builder;

namespace AspNetCore.MicroService.Swagger
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder UseSwagger(this IRouteBuilder routeBuilder)
        {
            return routeBuilder;
        }
    }
}