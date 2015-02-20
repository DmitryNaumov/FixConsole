namespace FixConsole
{
    public interface IRepository<T, TKey>
    {
        T GetById(TKey id);

        void AddNew(TKey id, T @object);
    }

    public interface IOrderRepository : IRepository<Order, string>
    {
    }
}