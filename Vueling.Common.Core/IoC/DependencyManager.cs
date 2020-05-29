using System;
using Ninject;
using Ninject.Modules;

namespace Vueling.Common.Core.IoC
{
    public class DependencyManager : IDisposable
    {
        #region Members
        private static DependencyManager instance = null;
        private static object lockobject = new object();
        private static object lockobject2 = new object();
        private IKernel container;
        private bool disposed = false;

        #endregion Members

        #region Constructor

        protected DependencyManager()
        {
        }

        public static DependencyManager Instance()
        {
            if (instance == null)
            {
                lock (lockobject)
                {
                    if (instance == null)
                        instance = new DependencyManager();
                }
            }
            return instance;
        }

        #endregion Constructor

        #region Methods

        public void Configure(INinjectModule[] configModules)
        {
            if (container == null)
            {
                lock (lockobject2)
                {
                    if (container == null)
                    {
                        container = new Ninject.StandardKernel(configModules);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IKernel GetKernel()
        {
            return container;
        }

        public void Release(object Instance)
        {
            container.Release(Instance);
        }

        public T Resolve<T>()
        {
            return container.Get<T>();
        }

        public object Resolve(Type serviceType)
        {
            return container.Get(serviceType);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                container.Dispose();
            }

            disposed = true;
        }

        #endregion Methods
    }
}
