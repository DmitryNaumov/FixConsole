using Autofac;

namespace FixConsole
{
    internal sealed class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ExecutionReportMessageHandler>().AsImplementedInterfaces();
            builder.RegisterType<InMemoryOrderRepository>().SingleInstance().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<MessageDispatcher>().SingleInstance().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<Application>().SingleInstance().AsSelf();
        }
    }
}