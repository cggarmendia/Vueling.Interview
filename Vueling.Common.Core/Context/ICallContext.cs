namespace Vueling.Common.Core.Context
{
    public interface ICallContext
    {
        #region Public Methods

        bool Contains(string Key);

        void Remove(string Key);

        T Retrieve<T>(string Key);

        T Retrieve<T>(string Key, T Default);

        void Save(string Key, object Obj);

        #endregion
    }
}
