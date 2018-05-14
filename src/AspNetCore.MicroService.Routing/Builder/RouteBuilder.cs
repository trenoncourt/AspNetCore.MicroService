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
        private readonly List<Action<Microsoft.AspNetCore.Routing.IRouteBuilder>> _routeBuilders = new List<Action<Microsoft.AspNetCore.Routing.IRouteBuilder>>();
        private readonly IApplicationBuilder _app;
        private readonly List<RouteBuilder> _allRoutes = new List<RouteBuilder>();
        
        public RouteBuilder(string template, IApplicationBuilder app)
        {
            Template = template;
            _app = app;
        }
        
        public RouteBuilder(string template, IApplicationBuilder app, List<RouteBuilder> chainedRoutes)
        {
            Template = template;
            _app = app;
            _allRoutes = chainedRoutes;
        }
        
        public string Template { get; }
        
        public dynamic Set { get; private set; }

        public IRouteBuilder Route(string template)
        {
            _allRoutes.Add(this);
            return new RouteBuilder(template, _app, _allRoutes);
        }

        public IRouteBuilder Route<T>(string template, ICollection<T> set)
        {
            _allRoutes.Add(this);
            return new RouteBuilder(template, _app, _allRoutes).AddSet(set);
        }
        
        public IRouteBuilder Get(Action<HttpContext> handler)
        {
            _routeBuilders.Add(builder =>
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
            _routeBuilders.Add(builder =>
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
            _routeBuilders.Add(builder =>
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
            _routeBuilders.Add(builder =>
            {
                builder.MapDelete(Template, async context =>
                {
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder AddSet<T>(ICollection<T> set)
        {
            Set = set;
            return this;
        }

        public IApplicationBuilder Use()
        {
            var routeBuilder = new Microsoft.AspNetCore.Routing.RouteBuilder(_app);
            
            foreach (RouteBuilder route in _allRoutes)
            {
                route.AddRoutes(routeBuilder);
            }
            
            AddRoutes(routeBuilder);

            return _app.UseRouter(routeBuilder.Build());
        }

        private void AddRoutes(Microsoft.AspNetCore.Routing.IRouteBuilder routeBuilder)
        {
            foreach (Action<Microsoft.AspNetCore.Routing.IRouteBuilder> addRoute in _routeBuilders)
            {
                addRoute(routeBuilder);
            }
        }
    }
}
#pragma warning restore 1998