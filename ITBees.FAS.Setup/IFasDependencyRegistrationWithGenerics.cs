using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITBees.FAS.Setup;

public interface IFasDependencyRegistrationWithGenerics
{
    void Register<TContext, TIdentityUser>(IServiceCollection services, IConfigurationRoot configurationRoot)
        where TContext : DbContext where TIdentityUser : IdentityUser, new();
}