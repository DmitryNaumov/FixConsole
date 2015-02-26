namespace FixConsole
{
    public class Execution
    {
        public string ExecutionId { get; set; }

        public Fix42.ExecType ExecType { get; set; }

        public double Qty { get; set; }

        public Price LastPx { get; set; }

        public Price AvgPx { get; set; }

        public double CumQty { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}@{2} - {3}", ExecType, Qty, LastPx, AvgPx);
        }
    }
}