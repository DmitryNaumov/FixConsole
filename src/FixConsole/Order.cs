using System;
using System.Collections.Generic;

namespace FixConsole
{
    public class Order
    {
        private const double Epsilon = Double.Epsilon;
        private Dictionary<string, Execution> _executions = new Dictionary<string, Execution>();

        public Order(string orderId)
        {
            OrderId = orderId;
        }

        public string OrderId { get; private set; }

        public bool Invalid { get; set; }

        public double CumQty { get; private set; }

        public void AddExecution(Execution execution)
        {
            switch (execution.ExecType)
            {
                case Fix42.ExecType.PartialFill:
                case Fix42.ExecType.Fill:
                    if (_executions.Count == 0 && Math.Abs(execution.CumQty - execution.Qty) > Epsilon)
                    {
                    }

                    _executions.Add(execution.ExecutionId, execution);
                    CumQty += execution.Qty;
                    break;
                case Fix42.ExecType.Canceled:
                    break;
                case Fix42.ExecType.New:
                case Fix42.ExecType.PendingCancel:
                case Fix42.ExecType.Rejected:
                    break;
                default:
                    break;
            }

        }

        public void CancelExecution(string executionId, Execution execution)
        {
            Execution cancelledExecution;
            if (!_executions.TryGetValue(executionId, out cancelledExecution))
                throw new InvalidProgramException();

            CumQty -= cancelledExecution.Qty;
            _executions.Add(execution.ExecutionId, execution);
        }

        public void CorrectExecution(string executionId, Execution execution)
        {
            Execution correctedExecution;
            if (!_executions.TryGetValue(executionId, out correctedExecution))
            {
                correctedExecution = new Execution
                {
                    ExecutionId = executionId,

                };
            }

            CumQty -= correctedExecution.Qty;
            
            _executions.Add(execution.ExecutionId, execution);
            CumQty += execution.Qty;
        }
    }
}