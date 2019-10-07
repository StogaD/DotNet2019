using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CoreWebApp.MediatR
{
    public class RequestHandler : IRequestHandler<RequestModel, string>
    {
        public Task<string> Handle(RequestModel request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"received {request.Message} !");
        }
    }
}
