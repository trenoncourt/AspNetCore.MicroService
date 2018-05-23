using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AspNetCore.MicroService.Routing.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using IRouteBuilder = AspNetCore.MicroService.Routing.Abstractions.Builder.IRouteBuilder;

namespace AspNetCore.MicroService.Routing.Builder
{
    
#pragma warning disable 1998
    public class RouteBuilder : IRouteBuilder
    {
        protected readonly List<Action<Microsoft.AspNetCore.Routing.IRouteBuilder>> RouteBuilders = new List<Action<Microsoft.AspNetCore.Routing.IRouteBuilder>>();
        protected readonly List<Action<HttpContext>> BeforeEachActions;
        private readonly List<IRouteBuilder> _allRoutes;

        public RouteBuilder(string template, IApplicationBuilder app, List<IRouteBuilder> chainedRoutes = null, List<Action<HttpContext>> beforeEachActions = null)
        {
            Template = template;
            App = app;
            _allRoutes = chainedRoutes ?? new List<IRouteBuilder>();
            BeforeEachActions = beforeEachActions ?? new List<Action<HttpContext>>();
            Settings = app.ApplicationServices.GetService<MicroServiceSettings>() ?? new MicroServiceSettings();
            Metadatas = app.ApplicationServices.GetService<MicroServiceMetadatas>();
        }
        
        public string Template { get; }

        public List<IRouteBuilder> AllRoutes => _allRoutes.Concat(new[] {this}).ToList();

        public MicroServiceSettings Settings { get; }
        
        public IApplicationBuilder App { get; }
        
        public MicroServiceMetadatas Metadatas { get; }

        public virtual IRouteBuilder Route(string template)
        {
            AllRoutes.Add(this);
            return new RouteBuilder(template, App, AllRoutes);
        }

        public IRouteBuilder SubRoute(string template)
        {
            AllRoutes.Add(this);
            return new RouteBuilder($"{Template}/{template.TrimStart('/')}", App, AllRoutes, BeforeEachActions);
        }

        public IRouteBuilder Get(Action<HttpContext> handler)
        {
            AddMetadatas(HttpMethods.Get);
            RouteBuilders.Add(builder =>
            {
                builder.MapGet(Template, async context =>
                {
                    BeforeEach(context);
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder Post(Action<HttpContext> handler)
        {
            AddMetadatas(HttpMethods.Post);
            RouteBuilders.Add(builder =>
            {
                builder.MapPost(Template, async context =>
                {
                    BeforeEach(context);
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder Put(Action<HttpContext> handler)
        {
            AddMetadatas(HttpMethods.Put);
            RouteBuilders.Add(builder =>
            {
                builder.MapPut(Template, async context =>
                {
                    BeforeEach(context);
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder Delete(Action<HttpContext> handler)
        {
            AddMetadatas(HttpMethods.Delete);
            RouteBuilders.Add(builder =>
            {
                builder.MapDelete(Template, async context =>
                {
                    BeforeEach(context);
                    handler(context);
                });
            });
            return this;
        }

        public IRouteBuilder BeforeEach(Action<HttpContext> handler)
        {
            BeforeEachActions.Add(handler);
            return this;
        }

        public IApplicationBuilder Use()
        {
            var routeBuilder = new Microsoft.AspNetCore.Routing.RouteBuilder(App);
            
            foreach (RouteBuilder route in AllRoutes)
            {
                route.AddRoutes(routeBuilder);
            }

            return App.UseRouter(routeBuilder.Build());
        }

        private void AddRoutes(Microsoft.AspNetCore.Routing.IRouteBuilder routeBuilder)
        {
            foreach (Action<Microsoft.AspNetCore.Routing.IRouteBuilder> addRoute in RouteBuilders)
            {
                addRoute(routeBuilder);
            }
        }

        private void AddMetadatas(string httpMethod)
        {
            if (!Settings.EnableMetadatas) return;

            var routeActionMetadata = new RouteActionMetadata
            {
                HttpMethod = httpMethod,
                RelativePath = Template
            };
            foreach (Match match in Regex.Matches(Template, "{(.*?)}"))
            {
                routeActionMetadata.Input.PathParameters.Add(new PathParameter
                {
                    Name = match.Groups[1].Value,
                    Type = typeof(string)
                }); 
            }
            Metadatas.RouteActionMetadatas.Add(routeActionMetadata);
        }
        
        protected void BeforeEach(HttpContext httpContext)
        {
            foreach (Action<HttpContext> action in BeforeEachActions)
            {
                action(httpContext);
            }
        }
    }
}
#pragma warning restore 1998