using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.MicroService.Routing.Builder
{
    
#pragma warning disable 1998
    public class RouteBuilder : IRouteBuilder
    {
        protected readonly List<Action<Microsoft.AspNetCore.Routing.IRouteBuilder>> RouteBuilders = new List<Action<Microsoft.AspNetCore.Routing.IRouteBuilder>>();
        protected readonly IApplicationBuilder App;
        protected readonly List<RouteBuilder> AllRoutes = new List<RouteBuilder>();
        
        public RouteBuilder(string template, IApplicationBuilder app)
        {
            Template = template;
            App = app;
        }
        
        public RouteBuilder(string template, IApplicationBuilder app, List<RouteBuilder> chainedRoutes)
        {
            Template = template;
            App = app;
            AllRoutes = chainedRoutes;
        }
        
        public string Template { get; }
        
        public virtual IRouteBuilder Route(string template)
        {
            AllRoutes.Add(this);
            return new RouteBuilder(template, App, AllRoutes);
        }
        
        public IRouteBuilder Get(Action<HttpContext> handler)
        {
            RouteBuilders.Add(builder =>
            {
                builder.MapGet(Template, async context =>
                {
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder Post(Action<HttpContext> handler)
        {
            RouteBuilders.Add(builder =>
            {
                builder.MapPost(Template, async context =>
                {
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder Put(Action<HttpContext> handler)
        {
            RouteBuilders.Add(builder =>
            {
                builder.MapPut(Template, async context =>
                {
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder Delete(Action<HttpContext> handler)
        {
            RouteBuilders.Add(builder =>
            {
                builder.MapDelete(Template, async context =>
                {
                    handler(context);
                });
            });
            return this;
        }

        public IApplicationBuilder Use()
        {
            var routeBuilder = new Microsoft.AspNetCore.Routing.RouteBuilder(App);
            
            foreach (RouteBuilder route in AllRoutes)
            {
                route.AddRoutes(routeBuilder);
            }
            
            AddRoutes(routeBuilder);

            return App.UseRouter(routeBuilder.Build());
        }

        private void AddRoutes(Microsoft.AspNetCore.Routing.IRouteBuilder routeBuilder)
        {
            foreach (Action<Microsoft.AspNetCore.Routing.IRouteBuilder> addRoute in RouteBuilders)
            {
                addRoute(routeBuilder);
            }
        }
    }
}
#pragma warning restore 1998