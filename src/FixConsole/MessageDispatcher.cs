using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace FixConsole
{
    internal sealed class MessageDispatcher
    {
        private static readonly int WorkerCount = 4;

        private readonly ILifetimeScope _container;

        private readonly ConcurrentQueue<FixMessage>[] _queues;
        private readonly Task[] _workers;

        public MessageDispatcher(ILifetimeScope container)
        {
            _container = container;
            _queues = Enumerable.Range(0, WorkerCount).Select(n => new ConcurrentQueue<FixMessage>()).ToArray();
            _workers = Enumerable.Range(0, _queues.Length).Select(n => Task.Factory.StartNew(() => Worker(_queues[n]))).ToArray();
        }

        public void Dispatch(FixMessage message)
        {
            if (message is ExecutionReportMessage)
            {
                uint n = (uint) ((uint) (message as ExecutionReportMessage).OrderId.GetHashCode()%_queues.Length);
                _queues[n].Enqueue(message);
            }
        }

        public bool IsEmpty()
        {
            return _queues.All(q => q.Count == 0);
        }

        private void Handle(ExecutionReportMessage message)
        {
            var handler = _container.Resolve<IFixMessageHandler<ExecutionReportMessage>>();
            handler.Handle(message);
        }

        private void Handle<T>(T message) where T : FixMessage
        {
            var handler = _container.Resolve<IFixMessageHandler<T>>();
            handler.Handle(message);
        }

        private void Worker(ConcurrentQueue<FixMessage> queue)
        {
            while (true)
            {
                FixMessage message = null;
                SpinWait.SpinUntil(() => queue.TryDequeue(out message));

                Handle(message as dynamic);
            }
        }
    }
}