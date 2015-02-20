using System.Collections.Generic;
using System.Data.SqlClient;

namespace FixConsole
{
    internal sealed class FixMessageSource
    {
        public IEnumerable<FixMessage> Read()
        {
            var parser = new FixMessageParser();

            using (var connection = new SqlConnection("Server=localhost;Database=PoMa_PKI;Trusted_Connection=True;MultipleActiveResultSets=true;"))
            //using (var connection = new SqlConnection("Server=localhost;Database=PoMa_NSR;Trusted_Connection=True;MultipleActiveResultSets=true;"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT message FROM [EOrderEventRawQueue] ORDER BY EOrderEventRawQueueID";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var value = reader.GetString(0);
                        var message = parser.Parse(value);
                        if (message != null)
                            yield return message;
                    }
                }
            }
        }
    }
}