using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using DryIoc;
using System;
using System.Reflection;

namespace Abp.Events.Bus
{
	/// <summary>
	/// Installs event bus system and registers all handlers automatically.
	/// </summary>
	internal class EventBusInstaller : IDryIocInstaller
	{
		private readonly IIocResolver _iocResolver;
		private readonly IEventBusConfiguration _eventBusConfiguration;
		private IEventBus _eventBus;

		public EventBusInstaller(IIocResolver iocResolver)
		{
			_iocResolver = iocResolver;
			_eventBusConfiguration = iocResolver.Resolve<IEventBusConfiguration>();
		}

		public void Install(IIocManager iocManager)
		{
			if (_eventBusConfiguration.UseDefaultEventBus)
			{
				iocManager.IocContainer.UseInstance<IEventBus>(EventBus.Default);
			}
			else
			{
				iocManager.IocContainer.Register<IEventBus, EventBus>(Reuse.Singleton);
			}

			_eventBus = iocManager.Resolve<IEventBus>();

			iocManager.RegisterTypeEventHandler += IocManager_RegisterTypeEventHandler;
		}

		private void IocManager_RegisterTypeEventHandler(IIocManager iocManager, Type registerType, Type implementationType)
		{
			/* This code checks if registering component implements any IEventHandler<TEventData> interface, if yes,
			 * gets all event handler interfaces and registers type to Event Bus for each handling event.
			 */
			if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(implementationType))
			{
				return;
			}

			var interfaces = implementationType.GetTypeInfo().GetInterfaces();
			foreach (var @interface in interfaces)
			{
				if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
				{
					continue;
				}

				var genericArgs = @interface.GetGenericArguments();
				if (genericArgs.Length == 1)
				{
					_eventBus.Register(genericArgs[0], new IocHandlerFactory(_iocResolver, implementationType));
				}
			}
		}
	}
}
