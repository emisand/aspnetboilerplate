using Abp.Application.Services;
using Abp.Extensions;
using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;

namespace Abp.Dependency
{
	public class AssemblyType
	{
		public Type ServiceType { get; set; }

		public Type ImplType { get; set; }
	}

	/// <summary>
	/// This class is used to register basic dependency implementations such as <see cref="ITransientDependency"/> and <see cref="ISingletonDependency"/>.
	/// </summary>
	public class BasicConventionalRegistrar : IConventionalDependencyRegistrar
	{
		public void RegisterAssembly(IConventionalRegistrationContext context)
		{
			// Transient Registration
			var waitRegisterTransient = GetTypes<ITransientDependency>(context.Assembly).ToList();

			foreach (var transientType in waitRegisterTransient)
			{
				if (typeof(IApplicationService).IsAssignableFrom(transientType.ImplType))
				{
					context.IocManager.Register(transientType.ServiceType, transientType.ImplType, DependencyLifeStyle.Transient, true);
					continue;
				}

				context.IocManager.RegisterIfNot(transientType.ServiceType, transientType.ImplType, DependencyLifeStyle.Transient);
			}

			// Singleton Registration
			var waitRegisterSingleton = GetTypes<ISingletonDependency>(context.Assembly).ToList();

			foreach (var singletonType in waitRegisterSingleton)
			{
				context.IocManager.RegisterIfNot(singletonType.ServiceType, singletonType.ImplType, DependencyLifeStyle.Singleton);
			}

			// Castle.DynamicProxy Interceptor Registration
			var waitRegisterInterceptor = GetTypes<IInterceptor>(context.Assembly).ToList();

			foreach (var interceptorType in waitRegisterInterceptor)
			{
				context.IocManager.RegisterIfNot(interceptorType.ServiceType, interceptorType.ImplType, DependencyLifeStyle.Transient);
			}
		}

		private ParallelQuery<AssemblyType> GetTypes<TInterface>(Assembly assembly)
		{
			Type GetServiceType(Type type)
			{
				var interfaces = type.GetInterfaces().Where(i => i != typeof(TInterface));

				// Prioritize interfaces by removing "I"
				var defaultInterface = interfaces.FirstOrDefault(i => type.Name.Equals(i.Name.RemovePreFix("I")));
				if (defaultInterface != null) return defaultInterface;
				if (interfaces.FirstOrDefault() != null) return interfaces.FirstOrDefault();
				return type;
			}

			return assembly.GetTypes()
				.AsParallel()
				.Where(type => typeof(TInterface).IsAssignableFrom(type))
				.Where(type => type.GetInterfaces().Any() && !type.IsInterface)
				.Where(type => !type.IsGenericTypeDefinition)
				.Where(type => !type.IsAbstract)
				.Select(type => new AssemblyType
				{
					ServiceType = GetServiceType(type),
					ImplType = type
				});
		}
	}
}