using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FixConsole
{
    internal sealed class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<string, Order> _orders = new ConcurrentDictionary<string, Order>(); 

        public Order GetById(string id)
        {
            Order order;
            if (!_orders.TryGetValue(id, out order))
                return null;

            return order;
        }

        public void AddNew(string id, Order order)
        {
            if (!_orders.TryAdd(id, order))
            {
                throw new InvalidProgramException();
            }
        }

        public long GetInvalidCount()
        {
            return _orders.Count(pair => pair.Value.Invalid);
        }
    }
}