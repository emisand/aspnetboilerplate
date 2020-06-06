using Abp.Dependency;
using DryIoc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Abp.EntityFrameworkCore.Configuration
{
	public class AbpEfCoreConfiguration : IAbpEfCoreConfiguration
	{
		private readonly IIocManager _iocManager;

		public AbpEfCoreConfiguration(IIocManager iocManager)
		{
			_iocManager = iocManager;
		}

		public void AddDbContext<TDbContext>(Action<AbpDbContextConfiguration<TDbContext>> action)
			where TDbContext : DbContext
		{
			_iocManager.IocContainer.UseInstance<IAbpDbContextConfigurer<TDbContext>>(new AbpDbContextConfigurerAction<TDbContext>(action));
		}
	}
}