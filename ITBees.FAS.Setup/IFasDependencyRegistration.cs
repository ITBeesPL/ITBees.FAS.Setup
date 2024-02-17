using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITBees.FAS.Setup;

public interface IFasDependencyRegistration
{
    void Register(IServiceCollection services, IConfigurationRoot configurationRoot);
}