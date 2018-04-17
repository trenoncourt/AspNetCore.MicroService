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
        
        public RouteBuilder(string template, IApplicationBuilder app)
        {
            Template = template;
            _app = app;
        }
        
        public string Template { get; }
        
        
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

        public IApplicationBuilder Use()
        {
            var routeBuilder = new Microsoft.AspNetCore.Routing.RouteBuilder(_app);

            foreach (Action<Microsoft.AspNetCore.Routing.IRouteBuilder> addRoute in _routeBuilders)
            {
                addRoute(routeBuilder);
            }

            return _app.UseRouter(routeBuilder.Build());
        }
    }
}
#pragma warning restore 1998