using System;

namespace AspNetCore.MicroService.Routing.Abstractions
{
    public enum InputLocation
    {
        Body,
        Query,
        Path
    }

    public static class InputLocationExtensions
    {
        public static string ToLowerString(this InputLocation inputLocation)
        {
            switch (inputLocation)
            {
                case InputLocation.Body:
                    return "body";
                case InputLocation.Query:
                    return "query";
                case InputLocation.Path:
                    return "path";
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputLocation), inputLocation, null);
            }
        } 
    }
}