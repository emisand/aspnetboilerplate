using System;
using System.Linq;
using Castle.DynamicProxy;
using DryIoc;
using ImTools;

public static class DryIocInterception
{
    static readonly DefaultProxyBuilder ProxyBuilder = new DefaultProxyBuilder();

    public static void Intercept(this IRegistrator registrator, Type serviceType, Type interceptorType, Type implType, object serviceKey = null)
    {
        // Determine whether the incoming type is an interface or a type in order to establish a proxy class
        Type proxyType;
        if (serviceType.IsInterface())
            proxyType = ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(
                serviceType, ArrayTools.Empty<Type>(), ProxyGenerationOptions.Default);
        else if (serviceType.IsClass())
            proxyType = ProxyBuilder.CreateClassProxyTypeWithTarget(
                serviceType, ArrayTools.Empty<Type>(), ProxyGenerationOptions.Default);
        else
            throw new ArgumentException(
                $"Intercepted service type {serviceType} is not a supported, cause it is nor a class nor an interface");

        // DryIoc decorator
        var decoratorSetup = serviceKey == null
            ? Setup.DecoratorWith(useDecorateeReuse: true)
            : Setup.DecoratorWith(r => serviceKey.Equals(r.ServiceKey), useDecorateeReuse: true);

        // Replace the original registration interface and resolve to the new proxy class
        registrator.Register(serviceType, proxyType,
            made: Made.Of((Type type) => type.GetConstructors().SingleOrDefault(c => c.GetParameters().Length != 0),
                Parameters.Of.Type<IInterceptor[]>(interceptorType.MakeArrayType()),
                // Added for atribute injection
                PropertiesAndFields.Auto),
            setup: decoratorSetup);
    }

    public static void Intercept<TService, TImplType, TInterceptor>(this IRegistrator registrator, object serviceKey = null)
        where TInterceptor : class, IInterceptor
    {
        Intercept(registrator, typeof(TService), typeof(TInterceptor), typeof(TImplType), serviceKey);
    }
}