﻿using Abp.Dependency;
using System;
using System.Linq;
using System.Reflection;

namespace Abp.EntityHistory
{
	internal static class EntityHistoryInterceptorRegistrar
	{
		public static void Initialize(IIocManager iocManager)
		{
			iocManager.RegisterTypeEventHandler += (manager, type, implementationType) =>
			{
				if (!iocManager.IsRegistered<IEntityHistoryConfiguration>())
				{
					return;
				}

				var entityHistoryConfiguration = iocManager.Resolve<IEntityHistoryConfiguration>();

				if (ShouldIntercept(entityHistoryConfiguration, implementationType))
				{
					iocManager.AddInterceptor(type, typeof(EntityHistoryInterceptor));
				}
			};
		}

		private static bool ShouldIntercept(IEntityHistoryConfiguration entityHistoryConfiguration, Type type)
		{
			if (type.GetTypeInfo().IsDefined(typeof(UseCaseAttribute), true))
			{
				return true;
			}

			if (type.GetMethods().Any(m => m.IsDefined(typeof(UseCaseAttribute), true)))
			{
				return true;
			}

			return false;
		}
	}
}
