namespace FixConsole
{
    public interface IFixMessageHandler<TMessage> where TMessage : FixMessage
    {
        void Handle(TMessage message);
    }
}