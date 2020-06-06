using System;
using DryIoc;

namespace Abp.Dependency
{
    /// <summary>
    /// This interface is used to directly perform dependency injection tasks.
    /// </summary>
    public interface IIocManager : IIocRegistrar, IIocResolver, IDisposable
    {
        /// <summary>
        /// Reference to the Castle Windsor Container.
        /// </summary>
        IContainer IocContainer { get; }

        /// <summary>
        /// Checks whether given type is registered before.
        /// </summary>
        /// <param name="type">Type to check</param>
        new bool IsRegistered(Type type);

        /// <summary>
        /// Checks whether given type is registered before.
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        new bool IsRegistered<T>();

        /// <summary>
        /// Event handler for Component Registration
        /// </summary>
        event RegisterTypeEventHandler RegisterTypeEventHandler;

        /// <summary>
        /// Initialize a container inside IocManager
        /// </summary>
        void InitializeInternalContainer(IContainer dryIocContainer);

        /// <summary>
        /// Child container
        /// </summary>
        /// <remarks>The value of this property is generally created by DryIocAdapter, and should not be assigned elsewhere</remarks>
        IResolverContext ChildContainer { get; }

        /// <summary>
        /// Initialize child container
        /// </summary>
        /// <param name="container">Used to initialize the subcontainer inside IocManager</param>
        void InitializeChildContainer(IResolverContext container);
    }
}