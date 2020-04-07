# unkillable-rmq

Small sample regarding the behavior of `ÌConnection.Close()` with a hanging consumer.

## How to

* Create a RabbitMQ server by running `docker-compose up -d`.
* Run the tests in your IDE (as 2 of them will hang, I advise you not to run them from the command line).

## Dependencies:
* NET.Core 3.1
* Docker

## Current Result

* `SynchronousTests.TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer` kills consumer
* `SynchronousTests.TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillKillConsumerAndNotRespectTimeout` kills consumer without respecting timeout
* `AsynchronousTests.TryingToKillConnection_WithAnHangingConsumer_WillWaitIndefinitely` waits indefinitely
* `AsynchronousTests.TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer` waits indefinitely

## Expected Result

* `SynchronousTests.TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer` waits for the consumer to finish (in this case, waits indefinitely)
* `SynchronousTests.TryingToKillConnectionWithTimeout_WithAnHangingConsumer_WillKillConsumerAndNotRespectTimeout` kills consumer if it is not finished before the timeout value
* `AsynchronousTests.TryingToKillConnection_WithAnHangingConsumer_WillWaitIndefinitely` waits for the consumer to finish (in this case, waits indefinitely)
* `AsynchronousTests.TryingToKillConnection_WithAnHangingConsumer_WillKillConsumer` kills consumer if it is not finished before the timeout value