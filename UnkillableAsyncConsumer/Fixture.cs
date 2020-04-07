using System;
using RabbitMQ.Client;

namespace UnkillableAsyncConsumer
{
    public class Fixture
    {
        private readonly ConnectionFactory _connectionFactory;

        public IConnection GetConnection => _connectionFactory.CreateConnection();
        public string Exchange { get; }

        public Fixture()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                UserName = "guest",
                Password = "guest",
                Port = 5772,
                DispatchConsumersAsync = true
            };

            Exchange = "RandomExchange";

            using var connection = _connectionFactory.CreateConnection();
            using var model = connection.CreateModel(); model.ExchangeDeclare(Exchange, ExchangeType.Direct, true, false);
        }
    }
}
