using System;
using System.Collections.Generic;
using System.Linq;

namespace FixConsole
{
    internal sealed class FixMessageParser : IFixMessageParser
    {
        public FixMessage Parse(string message)
        {
            var tags = ExtractTags(message).ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

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

        private IEnumerable<Tuple<int, string>> ExtractTags(string message)
        {
            int n = 0;
            while (true)
            {
                int k = message.IndexOf('=', n);
                int m = message.IndexOf('\x1', k);

                var s = message.Substring(n, k - n);

                int tag;
                if (Int32.TryParse(s, out tag))
                {

                    
                    if (m == -1)
                    {
                        yield return Tuple.Create(tag, message.Substring(k + 1));
                        yield break;
                    }

                    yield return Tuple.Create(tag, message.Substring(k + 1, m - k - 1));
                }

                n = m + 1;
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
            return new OrderCancelRejectMessage();
        }
    }
}