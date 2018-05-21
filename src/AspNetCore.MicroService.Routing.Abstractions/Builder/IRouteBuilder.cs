using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.MicroService.Routing.Abstractions.Builder
{
    public interface IRouteBuilder
    {
        string Template { get; }

        MicroServiceSettings Settings { get; }
        
        List<IRouteBuilder> AllRoutes { get; }
        
        IApplicationBuilder App { get; }
                
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