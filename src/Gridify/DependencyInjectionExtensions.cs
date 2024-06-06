using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Gridify;

/// <summary>
/// Extension methods for configuring Dependency Injection related to GridifyMapper.
/// </summary>
public static class DependencyInjectionExtensions
{
   /// <summary>
   /// Automatically scans an assembly for classes that inherit from GridifyMapper<T> and registers them in the Dependency Injection container.
   /// </summary>
   /// <param name="services">The IServiceCollection to which the mappers should be added.</param>
   /// <param name="assembly">The assembly to scan for GridifyMapper implementations.</param>
   /// <param name="lifetime">The service lifetime for the registered mappers (default is Singleton).</param>
   public static IServiceCollection AddGridifyMappers(this IServiceCollection services, Assembly assembly,
      ServiceLifetime lifetime = ServiceLifetime.Singleton)
   {
      var mapperType = typeof(GridifyMapper<>);

      var mapperTypes = assembly.GetTypes()
         .Where(type =>
            type is { IsAbstract: false, IsGenericTypeDefinition: false, BaseType.IsGenericType: true } &&
            type.BaseType.GetGenericTypeDefinition() == mapperType);

      foreach (var mapper in mapperTypes)
      {
         var genericArguments = mapper.BaseType?.GetGenericArguments();
         if (genericArguments is { Length: 1 })
         {
            var targetType = genericArguments[0];
var interfaceType = typeof(IGridifyMapper<>).MakeGenericType(targetType);
services.Add(new ServiceDescriptor(interfaceType, mapper, lifetime));
         }
      }
      return services;
   }
}
