using System;
using System.Diagnostics;
using System.Threading;

namespace FixConsole
{
    internal sealed class Application
    {
        private readonly MessageDispatcher _dispatcher;
        private readonly InMemoryOrderRepository _repository;

        public Application(MessageDispatcher dispatcher, InMemoryOrderRepository repository)
        {
            _dispatcher = dispatcher;
            _repository = repository;
        }

        public void Run()
        {
            var stopwatch = Stopwatch.StartNew();

            foreach (var message in new FixMessageSource().Read())
            {
                _dispatcher.Dispatch(message);
            }

            SpinWait.SpinUntil(() => _dispatcher.IsEmpty());
            var elapsed = stopwatch.Elapsed;
            Console.WriteLine(_repository.GetInvalidCount());
            Console.WriteLine(elapsed);
        }
    }
}