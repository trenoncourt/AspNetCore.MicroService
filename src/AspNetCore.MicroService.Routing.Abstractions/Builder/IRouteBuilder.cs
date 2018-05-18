using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.MicroService.Routing.Abstractions.Builder
{
    public interface IRouteBuilder
    {
        string Template { get; }
        
        IRouteBuilder Route(string template);
        
        IRouteBuilder SubRoute(string template);
        
        IRouteBuilder Get(Action<HttpContext> handler);
        
        IRouteBuilder Post(Action<HttpContext> handler);
        
        IRouteBuilder Put(Action<HttpContext> handler);
        
        IRouteBuilder Delete(Action<HttpContext> handler);

        IRouteBuilder BeforeEach(Action<HttpContext> handler);
        
        IApplicationBuilder Use();
    }
}