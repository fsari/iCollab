using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Audit;
using Owin;

namespace iCollab.Infra.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseSimpleLogger( this IAppBuilder app)
        {
            return app.Use<LoggingMiddleware>();
        }
    }
}