using Abp.Dependency;
using DryIoc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Reflection;

namespace Abp.AspNetCore
{
	public class AbpAspNetCoreConventionalRegistrar : IConventionalDependencyRegistrar
	{
		public void RegisterAssembly(IConventionalRegistrationContext context)
		{
			//Razor Pages
			var razorPageTypes = context.Assembly.GetTypes()
				.AsParallel()
				.Where(type => typeof(PageModel).IsAssignableFrom(type))
				.Where(type => !type.IsGenericTypeDefinition && !type.IsAbstract)
				.AsSequential();

			foreach (var type in razorPageTypes)
			{
				context.IocManager.Register(type, DependencyLifeStyle.Transient);
			}

			//ViewComponents
			var viewComponentTypes = context.Assembly.GetTypes()
				.AsParallel()
				.Where(type => typeof(ViewComponent).IsAssignableFrom(type))
				.Where(type => !type.IsGenericTypeDefinition)
				.AsSequential();

			foreach (var type in viewComponentTypes)
			{
				context.IocManager.Register(type, DependencyLifeStyle.Transient);
			}

			//PerWebRequest
			var perWebRequestClasses = context.Assembly.GetTypes()
				.AsParallel()
				.Where(type => type.IsClass)
				.Where(type => typeof(IPerWebRequestDependency).IsAssignableFrom(type))
				.Where(type => !type.GetTypeInfo().IsGenericTypeDefinition)
				.AsSequential();

			foreach (var type in perWebRequestClasses)
			{
				context.IocManager.IocContainer.Register(type, Reuse.Scoped, made: Made.Of(FactoryMethod.ConstructorWithResolvableArguments, propertiesAndFields: null));
			}
		}
	}
}
