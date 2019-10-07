using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CoreWebApp.MediatR
{
    public class OneWayRequestHandler : IRequestHandler<OneWayRequestModel>
    {
        public async Task<Unit> Handle(OneWayRequestModel request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Unit.Task.Result);

        
        }
    }
}
