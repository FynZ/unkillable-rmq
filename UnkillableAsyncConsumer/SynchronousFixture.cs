using RabbitMQ.Client;

namespace UnkillableAsyncConsumer
{
    public class SynchronousFixture
    {
        private readonly ConnectionFactory _connectionFactory;

        public IConnection GetConnection => _connectionFactory.CreateConnection();
        public string Exchange { get; }

        public SynchronousFixture()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                DispatchConsumersAsync = false
            };

            Exchange = "SynchronousExchange";

            using var connection = _connectionFactory.CreateConnection();
            using var model = connection.CreateModel(); model.ExchangeDeclare(Exchange, ExchangeType.Direct);
        }
    }
}
