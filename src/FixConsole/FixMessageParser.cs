using System;
using System.Collections.Generic;
using System.Linq;

namespace FixConsole
{
    internal sealed class FixMessageParser : IFixMessageParser
    {
        public FixMessage Parse(string message)
        {
            int value;
            var tags = message
                .Split('\x1')
                .Select(t => t.Split('='))
                .Where(pair => Int32.TryParse(pair[0], out value))
                .ToDictionary(pair => Int32.Parse(pair[0]), pair => pair[1]);

            switch (tags[Fix42.StandardHeader.MsgType])
            {
                case "8":
                    return ParseExecutionReport(tags);
                case "9":
                    return ParseOrderCancelReject(tags);
                default:
                    throw new NotImplementedException();
            }
        }

        private FixMessage ParseExecutionReport(Dictionary<int, string> tags)
        {
            var orderId = tags[Fix42.Fields.OrderID];
            //var clOrderId = tags[Fix42.Fields.ClOrdID];
            var executionId = tags[Fix42.Fields.ExecID];
            var executionTransactionType = tags[Fix42.Fields.ExecTransType].AsEnum<Fix42.ExecTransType>();
            var orderStatus = tags[Fix42.Fields.OrdStatus].ToOrdStatus();
            var symbol = tags[Fix42.Fields.Symbol];
            var side = tags[Fix42.Fields.Side];
            var orderQty = tags[Fix42.Fields.OrderQty];
            var leavesQty = tags[Fix42.Fields.LeavesQty].AsQty42();
            var cumQty = tags[Fix42.Fields.CumQty].AsQty42();
            var avgPx = tags[Fix42.Fields.AvgPx].AsPrice42();

            var lastShares = tags[Fix42.Fields.LastShares].AsQty42();

            var lastPx = tags.ContainsKey(Fix42.Fields.LastPx) ? tags[Fix42.Fields.LastPx].AsPrice42() : new Price(0);

            var execType = tags[Fix42.Fields.ExecType].ToExecType();

            return new ExecutionReportMessage(tags)
            {
                ExecutionId = executionId,
                OrderId = orderId,
                OrdStatus = orderStatus,

                CumQty = cumQty,
                AvgPx = avgPx,
                LeavesQty = leavesQty,

                LastShares = lastShares,
                LastPx = lastPx,
                ExecType = execType,
                ExecTransType = executionTransactionType
            };
        }

        private FixMessage ParseOrderCancelReject(Dictionary<int, string> tags)
        {
            return null;
        }
    }
}