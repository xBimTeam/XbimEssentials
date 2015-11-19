namespace Xbim.Common
{
    public interface IOptionalItemSet<T> : IItemSet<T>, IOptionalItemSet
    {
		void Initialize();
		void Uninitialize();
    }

	public interface IOptionalItemSet
    {
        bool Initialized { get; }
    }
}
