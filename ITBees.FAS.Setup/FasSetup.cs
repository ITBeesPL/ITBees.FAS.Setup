using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ITBees.FAS.Setup;

public class FasSetup
{
    /// <summary>
    /// That will scan all assemblyies in project and search for classes that implements IFasDependencyRegistration, and execute all dependencies
    /// </summary>
    public static void RegisterAllFasDependencies<YourDbContext, YourIdentityUser>(IServiceCollection services, IConfigurationRoot configurationRoot) where YourDbContext : DbContext where YourIdentityUser : IdentityUser<Guid>, new()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            if(assembly.FullName.StartsWith("Microsoft.AspNetCore.Server.IIS"))
                continue;
            
            var types = assembly.GetTypes();

            var registrationTypes = types.Where(t => typeof(IFasDependencyRegistration).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var type in registrationTypes)
            {
                var registrationInstance = (IFasDependencyRegistration)Activator.CreateInstance(type);
                registrationInstance.Register(services, configurationRoot);
            }

            var fasDependencyRegistrationWithGenerics = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFasDependencyRegistrationWithGenerics)))
                .ToList();

            foreach (var type in fasDependencyRegistrationWithGenerics)
            {
                MethodInfo method = type.GetMethod("Register");
                if (method != null)
                {
                    var genericMethod = method.MakeGenericMethod(typeof(YourDbContext), typeof(YourIdentityUser)); 
                    var instance = Activator.CreateInstance(type);
                    genericMethod.Invoke(instance, new object[] { services, configurationRoot });
                }
            }
        }
    }
}