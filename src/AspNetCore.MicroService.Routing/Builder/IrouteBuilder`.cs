using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.MicroService.Routing.Builder
{
    public interface IRouteBuilder<T> : IRouteBuilder
    {
        IEnumerable<T> Set { get; }

        IRouteBuilder<T> Route(string template, ICollection<T> set);
        
        IRouteBuilder<T> SubRoute(string template);

        new IRouteBuilder<T> Get(Action<HttpContext> handler);

        new IRouteBuilder<T> Post(Action<HttpContext> handler);

        new IRouteBuilder<T> Put(Action<HttpContext> handler);

        new IRouteBuilder<T> Delete(Action<HttpContext> handler);
    }
}