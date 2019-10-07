using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace CoreWebApp.MediatR
{
    public class EventHandler : INotificationHandler<EventModel>
    {
        private readonly ILogger _logger; 
        public EventHandler(ILogger logger)
        {
            _logger = logger;
        }      
        public Task Handle(EventModel notification, CancellationToken cancellationToken)
        {
            _logger.Information($"Notification Handler - received {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
