using System;

namespace AspNetCore.MicroService.Routing.Abstractions
{
    [Flags]
    public enum Methods
    {
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8
    }
}