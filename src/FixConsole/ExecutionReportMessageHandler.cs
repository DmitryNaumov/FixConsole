using System;

namespace FixConsole
{
    internal sealed class ExecutionReportMessageHandler : IFixMessageHandler<ExecutionReportMessage>
    {
        private readonly IOrderRepository _repository;

        public ExecutionReportMessageHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public void Handle(FixMessage message)
        {
            Handle(message as dynamic);
        }

        public void Handle(ExecutionReportMessage message)
        {
            var order = GetOrCreateOrder(message.OrderId);

            var execution = message.ToExecution();
            if (message.ExecTransType == Fix42.ExecTransType.New)
            {
                order.AddExecution(execution);
            }
            else if (message.ExecTransType == Fix42.ExecTransType.Cancel)
            {
                var cancelledExecutionId = message.GetValue(Fix42.Fields.ExecRefID);
                order.CancelExecution(cancelledExecutionId, execution);
            }
            else if (message.ExecTransType == Fix42.ExecTransType.Correct)
            {
                var cancelledExecutionId = message.GetValue(Fix42.Fields.ExecRefID);
                order.CorrectExecution(cancelledExecutionId, execution);
            }

            if (message.OrdStatus == Fix42.OrdStatus.Filled || message.OrdStatus == Fix42.OrdStatus.DoneForDay)
            {
                if (message.CumQty != order.CumQty)
                    order.Invalid = true;
            }

        }

        private Order GetOrCreateOrder(string orderId)
        {
            var order = _repository.GetById(orderId);
            if (order == null)
            {
                order = new Order(orderId);
                _repository.AddNew(orderId, order);
            }

            return order;
        }
    }
}