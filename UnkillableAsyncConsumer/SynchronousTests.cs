using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace UnkillableAsyncConsumer
{
    public class SynchronousTests : IClassFixture<SynchronousFixture>, IDisposable
    {
        private readonly SynchronousFixture _fixture;
        private readonly IConnection _connection;

        public SynchronousTests(SynchronousFixture fixture)
        {
            _fixture = fixture;
            _connection = fixture.GetConnection;
        }

        [Fact]
        public async Task TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer()
        {
            var queue = $"Synchronous-{Guid.NewGuid()}";

            using var model = _connection.CreateModel();
            var consumer = new EventingBasicConsumer(model);

            model.QueueDeclare(queue, true, false, false);
            model.QueueBind(queue, _fixture.Exchange, nameof(TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer));

            consumer.Received += (_, ea) =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }

                model.BasicAck(ea.DeliveryTag, true);
            };
            model.BasicConsume(queue: queue, autoAck: false, consumer: consumer);

            model.BasicPublish(_fixture.Exchange, nameof(TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer), null, Encoding.UTF8.GetBytes("Message"));

            await Task.Delay(5000);

            // close the connection without timeout, expected to never finish has the message processing never finishes
            // Consumer will be killed
            _connection.Close();

            Assert.True(true);
        }

        [Fact]
        public async Task TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillKillConsumerAndNotRespectTimeout()
        {
            var queue = $"Synchronous-Timeout-{Guid.NewGuid()}";

            using var model = _connection.CreateModel();
            var consumer = new EventingBasicConsumer(model);

            model.QueueDeclare(queue, true, false, false);
            model.QueueBind(queue, _fixture.Exchange, nameof(TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillKillConsumerAndNotRespectTimeout));

            consumer.Received += (_, ea) =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }

                model.BasicAck(ea.DeliveryTag, true);
            };
            model.BasicConsume(queue: queue, autoAck: false, consumer: consumer);


            model.BasicPublish(_fixture.Exchange, nameof(TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillKillConsumerAndNotRespectTimeout), null, Encoding.UTF8.GetBytes("Message"));

            await Task.Delay(5000);

            // close the connection with timeout, consumer is expected to be killed if the timeout passes
            // wait timeout is not respected and consumer will be killed before 5sec
            _connection.Close(5000);

            Assert.True(true);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
