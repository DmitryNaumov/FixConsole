using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Autofac;

namespace FixConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = BuildContainer())
            {
                container.Resolve<Application>().Run();

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());

            return builder.Build();
        }
    }

    public interface IFixMessageParser
    {
        FixMessage Parse(string message);
    }

    public static class Extensions
    {
        public static double AsQty42(this string value)
        {
            return Double.Parse(value, CultureInfo.InvariantCulture);
        }

        public static Price AsPrice42(this string value)
        {
            return Double.Parse(value, CultureInfo.InvariantCulture);
        }

        public static T AsEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof (T), value);
        }

        public static Fix42.OrdStatus ToOrdStatus(this string value)
        {
            switch (value)
            {
                case "A":
                    return Fix42.OrdStatus.PendingNew;
                case "E":
                    return Fix42.OrdStatus.PendingReplace;
                default:
                    return value.AsEnum<Fix42.OrdStatus>();
            }
        }

        public static Fix42.ExecType ToExecType(this string value)
        {
            switch (value)
            {
                case "A":
                    return Fix42.ExecType.PendingNew;
                case "E":
                    return Fix42.ExecType.PendingReplace;
                default:
                    return value.AsEnum<Fix42.ExecType>();
            }
        }
    }

    public abstract class FixMessage
    {
        public abstract Fix42.MsgType MsgType { get; }
    }

    public class OrderCancelRejectMessage : FixMessage
    {
        public override Fix42.MsgType MsgType
        {
            get { return Fix42.MsgType.CancelReject; }
        }
    }

    public class ExecutionReportMessage : FixMessage
    {
        private readonly Dictionary<int, string> _tags;

        public ExecutionReportMessage(Dictionary<int, string> tags)
        {
            _tags = tags;
        }

        public override Fix42.MsgType MsgType
        {
            get { return Fix42.MsgType.ExecutionReport; }
        }

        public string ExecutionId { get; set; }
        public string OrderId { get; set; }
        public Fix42.OrdStatus OrdStatus { get; set; }

        public double LastShares { get; set; }
        public Price LastPx { get; set; }
        public Fix42.ExecType ExecType { get; set; }
        public Fix42.ExecTransType ExecTransType { get; set; }

        public double CumQty { get; set; }
        public Price AvgPx { get; set; }
        public double LeavesQty { get; set; }

        public string GetValue(int tag)
        {
            return _tags[tag];
        }

    }

    public struct Price
    {
        private readonly double _value;

        public Price(double value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static implicit operator Price(double value)
        {
            return new Price(value);
        }
    }

    public static class Fix42
    {
        public static class StandardHeader
        {
            public static readonly int MsgType = 35;
        }

        public static class Fields
        {
            public static readonly int OrderID = 37;
            public static readonly int ClOrdID = 11;
            public static readonly int ExecID = 17;
            public static readonly int ExecTransType = 20;
            public static readonly int OrdStatus = 39;
            public static readonly int Symbol = 55;
            public static readonly int Side = 54;
            public static readonly int OrderQty = 38;
            public static readonly int LeavesQty = 151;
            public static readonly int CumQty = 14;
            public static readonly int AvgPx = 6;

            public static readonly int LastPx = 31;
            public static readonly int LastShares = 32;
            public static readonly int ExecType = 150;

            public static readonly int ExecRefID = 19;
        }

        public enum MsgType
        {
            ExecutionReport = 8,

            CancelReject = 12345 // ?!
        }

        public enum OrdStatus
        {
            New = 0,
            PartiallyFilled = 1,
            Filled = 2,
            DoneForDay = 3,

            PendingNew = 0xA,
            PendingReplace = 0xE
            // TODO
        }

        public enum ExecType
        {
            New = 0,
            PartialFill = 1,
            Fill = 2,
            DoneForDay = 3,
            Canceled = 4,

            PendingCancel = 6,
            Rejected = 8,

            PendingNew = 0xA,
            PendingReplace = 0xE
            // TODO
        }

        public enum ExecTransType
        {
            New = 0,
            Cancel = 1,
            Correct = 2,
            Status = 3
        }
    }
}
