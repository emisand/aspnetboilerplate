using Abp.Application.Services;
using Abp.Dependency;
using System.Reflection;

namespace Abp.Runtime.Validation.Interception
{
	internal static class ValidationInterceptorRegistrar
	{
		public static void Initialize(IIocManager iocManager)
		{
			iocManager.RegisterTypeEventHandler += (manager, type, implementationType) =>
			{
				if (typeof(IApplicationService).GetTypeInfo().IsAssignableFrom(implementationType))
				{
					manager.AddInterceptor(type, typeof(ValidationInterceptor));
				}
			};
		}
	}
}