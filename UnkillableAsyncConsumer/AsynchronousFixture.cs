using RabbitMQ.Client;

namespace UnkillableAsyncConsumer
{
    public class AsynchronousFixture
    {
        private readonly ConnectionFactory _connectionFactory;

        public IConnection GetConnection => _connectionFactory.CreateConnection();
        public string Exchange { get; }

        public AsynchronousFixture()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                DispatchConsumersAsync = true
            };

            Exchange = "AsynchronousExchange";

            using var connection = _connectionFactory.CreateConnection();
            using var model = connection.CreateModel(); model.ExchangeDeclare(Exchange, ExchangeType.Direct);
        }
    }
}
