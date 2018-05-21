using System;
using System.Collections.Generic;
using AspNetCore.MicroService.Routing.Abstractions.Builder;
using AspNetCore.MicroService.Routing.Metadatas;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.MicroService.Routing.Builder
{
    public class RouteBuilder<T> : RouteBuilder, IRouteBuilder<T>
    {

        public IEnumerable<T> Set { get; }
        
        public RouteBuilder(string template, IApplicationBuilder app, IEnumerable<T> set) : base(template, app)
        {
            Set = set;
        }

        public RouteBuilder(string template, IApplicationBuilder app, List<Abstractions.Builder.IRouteBuilder> chainedRoutes, IEnumerable<T> set) : base(template, app, chainedRoutes)
        {
            Set = set;
        }

        public IRouteBuilder<T> Route(string template, IEnumerable<T> set)
        {
            AllRoutes.Add(this);
            return new RouteBuilder<T>(template, App, AllRoutes, set);
        }

        public IRouteBuilder<T> SubRoute(string template)
        {
            AllRoutes.Add(this);
            return new RouteBuilder<T>($"{Template}/{template.TrimStart('/')}", App, AllRoutes, Set);
        }

        public new IRouteBuilder<T> Get(Action<HttpContext> handler)
        {
            AddMetadatas("GET");
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

        public new IRouteBuilder<T> Post(Action<HttpContext> handler)
        {
            AddMetadatas("POST");
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

        public new IRouteBuilder<T> Put(Action<HttpContext> handler)
        {
            AddMetadatas("PUT");
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

        public new IRouteBuilder<T> Delete(Action<HttpContext> handler)
        {
            AddMetadatas("DELETE");
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

        public new IRouteBuilder<T> BeforeEach(Action<HttpContext> handler)
        {
            BeforeEachActions.Add(handler);
            return this;
        }

        private void AddMetadatas(string httpMethod)
        {
            this.AddMetadata(metadatas =>
            {
                metadatas.HttpMethod = httpMethod;
                metadatas.RelativePath = Template;
                metadatas.ReturnType = typeof(T);
                metadatas.InputType = typeof(T);
            });
        }
    }
}