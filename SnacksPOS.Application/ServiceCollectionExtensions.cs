using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;

namespace SnacksPOS.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        return services;
    }
}
