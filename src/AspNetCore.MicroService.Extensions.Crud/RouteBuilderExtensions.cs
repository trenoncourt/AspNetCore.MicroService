using System.Collections.Generic;
using AspNetCore.MicroService.Extensions.Json;
using AspNetCore.MicroService.Routing.Builder;

namespace AspNetCore.MicroService.Extensions.Crud
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder Get<T>(this IRouteBuilder routeBuilder, IEnumerable<T> set)
        {
            return routeBuilder.Get(async c => await c.WriteJsonAsync(set));
        }
        
        public static IRouteBuilder Post<T>(this IRouteBuilder routeBuilder, ICollection<T> set)
        {
            return routeBuilder.Post(async c =>
            {
                var o = c.ReadJsonBody<T>();
                if (o != null)
                {
                    set.Add(o);
                    c.Response.Headers["Location"] = $"/{routeBuilder.Template}/{typeof(T)?.GetType()?.GetProperty("Id")?.GetValue(o) ?? ""}";
                    c.Response.StatusCode = 201;
                }
                else c.Response.StatusCode = 400;
            });
        }
    }
}