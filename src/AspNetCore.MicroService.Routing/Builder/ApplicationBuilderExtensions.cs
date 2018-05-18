using System;
using System.Collections.Generic;
using AspNetCore.MicroService.Routing.Abstractions.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.MicroService.Routing.Builder
{
    /// <summary>
    /// Extension methods for the <see cref="T:Microsoft.AspNetCore.Builder.Extensions.MapMiddleware" />.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        private const string PathMatchKey = "Trenoncourt_PathMatch";
        
        /// <summary>
        /// Branches the request pipeline based on matches of the given request path. If the request path starts with
        /// the given path, the branch is executed.
        /// </summary>
        /// <param name="app">The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <returns>The new <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> instance.</returns>
        public static IApplicationBuilder Map(this IApplicationBuilder app, PathString pathMatch)
        {
            if (app == null)
                throw new ArgumentNullException(nameof (app));
            if (pathMatch.HasValue && pathMatch.Value.EndsWith("/", StringComparison.Ordinal))
                throw new ArgumentException("The path must not end with a '/'", nameof (pathMatch));

            IApplicationBuilder applicationBuilder = app.New();
            applicationBuilder.Properties.Add(PathMatchKey, pathMatch);

            return applicationBuilder;
        }
        
        /// <summary>
        /// Define a new route using routebuilder with template.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static IRouteBuilder Route(this IApplicationBuilder app, string template)
        {
            return new RouteBuilder(template, app);
        }

        /// <summary>
        /// Define a new route using routebuilder with template.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="template"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static IRouteBuilder<T> Route<T>(this IApplicationBuilder app, string template, IEnumerable<T> set)
        {
            return new RouteBuilder<T>(template, app, set);
        }
        
        /// <summary>
        /// Build the request pipeline based on matches of the given request path.
        /// </summary>
        /// <param name="app">The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> instance.</param>
        /// <param name="pathMatch">The request path to match.</param>
        /// <returns>The <see cref="T:Microsoft.AspNetCore.Builder.IApplicationBuilder" /> instance.</returns>
        public static IApplicationBuilder Use(this IApplicationBuilder app)
        {
            object pathMatch = app.Properties[PathMatchKey];
            if (pathMatch == null)
                return app;
            
            RequestDelegate requestDelegate = app.Build();
            MapOptions options = new MapOptions
            {
                Branch = requestDelegate,
                PathMatch = (PathString)pathMatch
            };
            return app.Use(next => new MapMiddleware(next, options).Invoke);
        }
    }
}