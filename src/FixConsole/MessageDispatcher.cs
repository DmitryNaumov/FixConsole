using Autofac;

namespace FixConsole
{
    internal sealed class MessageDispatcher
    {
        private readonly ILifetimeScope _container;

        public MessageDispatcher(ILifetimeScope container)
        {
            _container = container;
        }

        public void Dispatch(FixMessage message)
        {
            Handle(message as dynamic);
        }

        private void Handle<T>(T message) where T : FixMessage
        {
            var handler = _container.Resolve<IFixMessageHandler<T>>();
            handler.Handle(message);
        }
    }
}