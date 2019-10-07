using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace CoreWebApp.MediatR
{
    public class RequestModel : IRequest<string>
    {
        public string Message { get; set; }
    }
}
