using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Logging.Persistence;

namespace Orchestrix.Coordinator;

internal class LoggingConfigurationBuilder(IServiceCollection services) : ILoggingConfigurationBuilder
{
    public IServiceCollection Services => services;
}
