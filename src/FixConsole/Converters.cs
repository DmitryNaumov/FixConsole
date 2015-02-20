namespace FixConsole
{
    public static class Converters
    {
        public static Execution ToExecution(this ExecutionReportMessage message)
        {
            return new Execution
            {
                ExecutionId = message.ExecutionId,
                ExecType = message.ExecType,

                AvgPx = message.AvgPx,
                Qty = message.LastShares,
                CumQty = message.CumQty
            };
        }
    }
}