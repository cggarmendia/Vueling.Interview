using System;
using System.Web;
using Vueling.Common.Core.Context;

namespace Vueling.Common.Presentation.API
{
    public class WebCallContext : ICallContext
    {
        #region Ctor.

        public WebCallContext()
        {
        }

        #endregion Constructor

        #region Methods

        public bool Contains(string Key)
        {
            return HttpContext.Current.Items.Contains(Key);
        }

        public void Remove(string Key)
        {
            HttpContext.Current.Items.Remove(Key);
        }

        public T Retrieve<T>(string Key)
        {
            lock (HttpContext.Current.Items)
            {
                if (!HttpContext.Current.Items.Contains(Key))
                    throw new InvalidOperationException("Key not found");
                return (T)HttpContext.Current.Items[Key];
            }
        }

        public T Retrieve<T>(string Key, T Default)
        {
            lock (HttpContext.Current.Items)
            {
                if (HttpContext.Current.Items.Contains(Key))
                    return (T)HttpContext.Current.Items[Key];
                return Default;
            }
        }

        public void Save(string Key, object Obj)
        {
            HttpContext.Current.Items[Key] = Obj;
        }

        #endregion Methods
    }
}
