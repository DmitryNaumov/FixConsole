using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FixConsole
{
    internal sealed class FixMessageSource
    {
        private readonly FixMessageParser _parser = new FixMessageParser();

        public IEnumerable<FixMessage> Read()
        {
            using (var connection = new SqlConnection("Server=localhost;Database=PoMa_PKI;Trusted_Connection=True;MultipleActiveResultSets=true;"))
            //using (var connection = new SqlConnection("Server=localhost;Database=PoMa_NSR;Trusted_Connection=True;MultipleActiveResultSets=true;"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT message FROM [EOrderEventRawQueue] ORDER BY EOrderEventRawQueueID";
                using (var reader = command.ExecuteReader())
                {
                    while (true)
                    {
                        var tasks = Enumerable.Range(0, 4).Select(n => ReadAndParse(reader)).ToArray();

                        foreach (var task in tasks)
                        {
                            if (task.Result == null)
                                yield break;

                            yield return task.Result;
                        }
                    }
                }
            }
        }

        private Task<FixMessage> ReadAndParse(SqlDataReader reader)
        {
            if (!reader.Read())
                return Task.FromResult<FixMessage>(null);

            var value = reader.GetString(0);
            return Task.Factory.StartNew(() => _parser.Parse(value));
        }
    }
}