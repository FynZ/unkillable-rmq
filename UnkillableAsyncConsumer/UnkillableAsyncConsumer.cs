using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace UnkillableAsyncConsumer
{
    public class UnkillableAsyncConsumer : IClassFixture<Fixture>, IDisposable
    {
        private readonly Fixture _fixture;
        private readonly IConnection _connection;

        public UnkillableAsyncConsumer(Fixture fixture)
        {
            _fixture = fixture;
            _connection = fixture.GetConnection;
        }

        [Fact]
        public async Task TryingToKillConnection_WithAnHangingConsumer_WillWaitIndefinitely()
        {
            var queue = Guid.NewGuid().ToString();

            using var model = _connection.CreateModel();
            var consumer = new AsyncEventingBasicConsumer(model);

            model.QueueDeclare(queue, true, false);
            model.QueueBind(queue, _fixture.Exchange, nameof(TryingToKillConnection_WithAnHangingConsumer_WillWaitIndefinitely));

            consumer.Received += async (_, ea) =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                }

                model.BasicAck(ea.DeliveryTag, true);
            };
            model.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

            model.BasicPublish(_fixture.Exchange, nameof(TryingToKillConnection_WithAnHangingConsumer_WillWaitIndefinitely), null, Encoding.UTF8.GetBytes("Message"));

            await Task.Delay(5000);

            // close the connection without timeout, expected to never finish has the message processing never finishes
            _connection.Close();

            Assert.True(true);
        }

        [Fact]
        public async Task TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillWaitIndefinitely()
        {
            var queue = Guid.NewGuid().ToString();

            using var model = _connection.CreateModel();
            var consumer = new AsyncEventingBasicConsumer(model);

            model.QueueDeclare(queue, true, false);
            model.QueueBind(queue, _fixture.Exchange, nameof(TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillWaitIndefinitely));

            consumer.Received += async (_, ea) =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                }

                model.BasicAck(ea.DeliveryTag, true);
            };
            model.BasicConsume(queue: queue, autoAck: false, consumer: consumer);


            model.BasicPublish(_fixture.Exchange, nameof(TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillWaitIndefinitely), null, Encoding.UTF8.GetBytes("Message"));

            await Task.Delay(5000);

            // close the connection with timeout, consumer is expected to be killed if the timeout passes
            _connection.Close(1000);

            Assert.True(true);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
