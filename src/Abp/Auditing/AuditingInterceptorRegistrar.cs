﻿using Abp.Dependency;
using System;
using System.Linq;
using System.Reflection;

namespace Abp.Auditing
{
	internal static class AuditingInterceptorRegistrar
	{
		public static void Initialize(IIocManager iocManager)
		{
			iocManager.RegisterTypeEventHandler += (manager, type, implementationType) =>
			{
				if (!iocManager.IsRegistered<IAuditingConfiguration>())
				{
					return;
				}

				var auditingConfiguration = iocManager.Resolve<IAuditingConfiguration>();

				if (ShouldIntercept(auditingConfiguration, implementationType))
				{
					iocManager.AddAsyncInterceptor(type, typeof(AuditingInterceptor));
				}
			};
		}

		private static bool ShouldIntercept(IAuditingConfiguration auditingConfiguration, Type type)
		{
			if (auditingConfiguration.Selectors.Any(selector => selector.Predicate(type)))
			{
				return true;
			}

			if (type.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
			{
				return true;
			}

			if (type.GetMethods().Any(m => m.IsDefined(typeof(AuditedAttribute), true)))
			{
				return true;
			}

			return false;
		}
	}
}