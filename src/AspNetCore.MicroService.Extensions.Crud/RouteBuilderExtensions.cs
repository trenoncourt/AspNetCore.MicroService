using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AspNetCore.MicroService.Extensions.Json;
using AspNetCore.MicroService.Routing;
using AspNetCore.MicroService.Routing.Abstractions;
using AspNetCore.MicroService.Routing.Abstractions.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using IRouteBuilder = AspNetCore.MicroService.Routing.Abstractions.Builder.IRouteBuilder;

namespace AspNetCore.MicroService.Extensions.Crud
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder Get<T>(this IRouteBuilder routeBuilder, IEnumerable<T> set)
        {
            return routeBuilder.Get(async c =>
            {
                int routeDataCount = c.GetRouteData().Values.Count;
                if (routeDataCount == 0)
                {
                    await c.WriteJsonAsync(set);
                    return;
                }

                await Get(c, set, c.GetRouteData().Values.First().Key);
            });
        }
        public static IRouteBuilder<T> Get<T>(this IRouteBuilder<T> routeBuilder)
        {
            return routeBuilder.Get(async c =>
            {
                int routeDataCount = c.GetRouteData().Values.Count;
                if (routeDataCount == 0)
                {
                    await c.WriteJsonAsync(routeBuilder.Set);
                    return;
                }

                await Get(c, routeBuilder.Set, c.GetRouteData().Values.First().Key);
            });
        }
        
        public static IRouteBuilder Get<T>(this IRouteBuilder routeBuilder, IEnumerable<T> set, string id)
        {
            return routeBuilder.Get(async c => { await Get(c, set, id); });
        }
        
        public static IRouteBuilder Post<T>(this IRouteBuilder routeBuilder, ICollection<T> set)
        {
            return routeBuilder.Post(c =>
            {
                Post(c, routeBuilder, set);
            });
        }
        
        public static IRouteBuilder<T> Post<T>(this IRouteBuilder<T> routeBuilder)
        {
            return routeBuilder.Post(c =>
            {
                Post(c, routeBuilder, routeBuilder.Set as ICollection<T>);
            });
        }
        
        public static IRouteBuilder Put<T>(this IRouteBuilder routeBuilder, ICollection<T> set)
        {
            return routeBuilder.Put(c =>
            {
                string key = c.GetRouteData().Values.First().Key;
                Put(c, set, key);
            });
        }
        
        public static IRouteBuilder Put<T>(this IRouteBuilder routeBuilder, ICollection<T> set, string id)
        {
            return routeBuilder.Put(c => { Put(c, set, id); });
        }
        
        public static IRouteBuilder<T> Put<T>(this IRouteBuilder<T> routeBuilder)
        {
            return routeBuilder.Put(c =>
            {
                string key = c.GetRouteData().Values.First().Key;
                Put(c, routeBuilder.Set as ICollection<T>, key);
            });
        }
        
        public static IRouteBuilder<T> Put<T>(this IRouteBuilder<T> routeBuilder, string id)
        {
            return routeBuilder.Put(c => { Put(c, routeBuilder.Set as ICollection<T>, id); });
        }
        
        public static IRouteBuilder Delete<T>(this IRouteBuilder routeBuilder, ICollection<T> set)
        {
            return routeBuilder.Delete(c =>
            {
                string key = c.GetRouteData().Values.First().Key;
                Delete(c, set, key);
            });
        }
        
        public static IRouteBuilder Delete<T>(this IRouteBuilder routeBuilder, ICollection<T> set, string id)
        {
            return routeBuilder.Delete(c => { Delete(c, set, id); });
        }
        
        public static IRouteBuilder<T> Delete<T>(this IRouteBuilder<T> routeBuilder)
        {
            return routeBuilder.Delete(c =>
            {
                string key = c.GetRouteData().Values.First().Key;
                Delete(c, routeBuilder.Set as ICollection<T>, key);
            });
        }
        
        public static IRouteBuilder<T> Delete<T>(this IRouteBuilder<T> routeBuilder, string id)
        {
            return routeBuilder.Delete(c => { Delete(c, routeBuilder.Set as ICollection<T>, id); });
        }

        public static IRouteBuilder<T> Crud<T>(this IRouteBuilder<T> routeBuilder, Methods methods = Methods.Get | Methods.Post | Methods.Put | Methods.Delete, string id = "id")
        {
            if (methods.HasFlag(Methods.Get))
            {
                routeBuilder.Get();
            }
            if (methods.HasFlag(Methods.Post))
            {
                routeBuilder.Post();
            }

            if(methods.HasFlag(Methods.Get | Methods.Put | Methods.Delete))
            {
                var newRouteBuilder = routeBuilder.SubRoute($"{{id}}");
                if (methods.HasFlag(Methods.Get))
                {
                    newRouteBuilder.Get();
                }
                if (methods.HasFlag(Methods.Put))
                {
                    newRouteBuilder.Put();
                }
                if (methods.HasFlag(Methods.Delete))
                {
                    newRouteBuilder.Delete();
                }
                return newRouteBuilder.Route(routeBuilder.Template, routeBuilder.Set);
            }
            return routeBuilder;
        }

        private static Task Get<T>(HttpContext c, IEnumerable<T> set, string id)
        {
            T o = set.FirstOrDefault(x => typeof(T).GetProperty("Id")?.GetValue(x).ToString() == c.GetRouteValue(id).ToString());
            if (o == null)
            {
                c.Response.StatusCode = 404;
                return Task.CompletedTask;
            }
            return c.WriteJsonAsync(o);
        }

        private static void Post<T>(HttpContext c, IRouteBuilder routeBuilder, ICollection<T> set)
        {
            var o = c.ReadJsonBody<T>();
            if (o != null)
            {
                set.Add(o);
                c.Response.Headers["Location"] = $"/{routeBuilder.Template}/{typeof(T).GetProperty("Id")?.GetValue(o) ?? ""}";
                c.Response.StatusCode = 201;
            }
            else c.Response.StatusCode = 400;
        }

        private static void Put<T>(HttpContext c, IEnumerable<T> set, string id)
        {
            string idValue = c.GetRouteValue(id).ToString();
            T existing = set.FirstOrDefault(x => typeof(T).GetProperty("Id")?.GetValue(x).ToString().ToString() == idValue);
            if (existing == null)
            {
                c.Response.StatusCode = 404;
                return;
            }
            var o = c.ReadJsonBody<T>();
            if (typeof(T).GetProperty("Id")?.GetValue(o).ToString() != idValue)
            {
                c.Response.StatusCode = 400;
                return;
            }
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.PropertyType.IsPrimitive 
                    || property.PropertyType == typeof(Decimal) 
                    || property.PropertyType == typeof(TimeSpan) 
                    || property.PropertyType == typeof(DateTimeOffset) 
                    || property.PropertyType == typeof(Guid) 
                    || property.PropertyType == typeof(string)
                    || property.Name == "Id")
                {
                    property.SetValue(existing, typeof(T).GetProperty(property.Name)?.GetValue(o));
                }
            }
            c.Response.StatusCode = 204;
        }

        private static void Delete<T>(HttpContext c, ICollection<T> set, string id)
        {
            T o = set.FirstOrDefault(x => typeof(T).GetProperty("Id")?.GetValue(x).ToString() == c.GetRouteValue(id).ToString());
            if (o == null)
            {
                c.Response.StatusCode = 404;
                return;
            }
            set.Remove(o);
            c.Response.StatusCode = 204;
        }
    }
}