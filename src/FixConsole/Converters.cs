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

                LastPx = message.LastPx,
                Qty = message.LastShares,
                AvgPx = message.AvgPx,
                CumQty = message.CumQty
            };
        }
    }
}