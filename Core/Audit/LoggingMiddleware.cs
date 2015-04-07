using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Core.Audit
{
    public class LoggingMiddleware : OwinMiddleware
    { 
        public LoggingMiddleware(OwinMiddleware next): base(next)
        { 
        }

        public async override Task Invoke(IOwinContext context)
        {
            Console.WriteLine("Begin Request");
            await Next.Invoke(context);
            Console.WriteLine("End Request");
        }
    }
}
