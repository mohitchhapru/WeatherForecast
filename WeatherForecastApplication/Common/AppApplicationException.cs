using System;
using System.Net;

namespace WeatherForecastApplication.Common
{
    public class AppApplicationException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorMessage { get; set; }

        public AppApplicationException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorMessage = message;
        }
    }
}