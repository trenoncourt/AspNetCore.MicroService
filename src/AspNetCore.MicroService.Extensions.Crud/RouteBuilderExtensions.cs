using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AspNetCore.MicroService.Extensions.Json;
using Microsoft.AspNetCore.Routing;
using IRouteBuilder = AspNetCore.MicroService.Routing.Builder.IRouteBuilder;

namespace AspNetCore.MicroService.Extensions.Crud
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder Get<T>(this IRouteBuilder routeBuilder, IEnumerable<T> set)
        {
            return routeBuilder.Get(async c => await c.WriteJsonAsync(set));
        }
        
        public static IRouteBuilder Get<T>(this IRouteBuilder routeBuilder, IEnumerable<T> set, string id)
        {
            return routeBuilder.Get(async c =>
            {
                T o = set.FirstOrDefault(x => typeof(T)?.GetType()?.GetProperty("Id")?.GetValue(x).ToString() == c.GetRouteValue(id).ToString());
                if (o == null) c.Response.StatusCode = 404;
                else
                    await c.WriteJsonAsync(o);
            });
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
        
        public static IRouteBuilder Put<T>(this IRouteBuilder routeBuilder, ICollection<T> set, string id)
        {
            return routeBuilder.Post(async c =>
            {
                string idValue = c.GetRouteValue(id).ToString();
                T existing = set.FirstOrDefault(x => typeof(T)?.GetType()?.GetProperty("Id")?.GetValue(x).ToString().ToString() == idValue);
                if (existing == null)
                {
                    c.Response.StatusCode = 404;
                    return;
                }
                var o = c.ReadJsonBody<T>();
                if (typeof(T)?.GetType()?.GetProperty("Id")?.GetValue(o).ToString() != idValue)
                {
                    c.Response.StatusCode = 400;
                    return;
                }
                foreach (PropertyInfo property in typeof(T)?.GetType()?.GetProperties(BindingFlags.Public))
                {
                    if (property.PropertyType.IsPrimitive 
                        || property.PropertyType == typeof(Decimal) 
                        || property.PropertyType == typeof(TimeSpan) 
                        || property.PropertyType == typeof(DateTimeOffset) 
                        || property.PropertyType == typeof(Guid) 
                        || property.PropertyType == typeof(string)
                        || property.Name == "Id")
                    {
                        continue;
                    }
                    property.SetValue(existing, typeof(T)?.GetType()?.GetProperty(property.Name)?.GetValue(o));
                }
                c.Response.StatusCode = 204;
            });
        }
        
        public static IRouteBuilder Delete<T>(this IRouteBuilder routeBuilder, ICollection<T> set, string id)
        {
            return routeBuilder.Delete(async c =>
            {
                T o = set.FirstOrDefault(x => typeof(T)?.GetType()?.GetProperty("Id")?.GetValue(x).ToString() == c.GetRouteValue(id).ToString());
                if (o == null)
                {
                    c.Response.StatusCode = 404;
                    return;
                }
                set.Remove(o);
                c.Response.StatusCode = 204;
            });
        }
    }
}