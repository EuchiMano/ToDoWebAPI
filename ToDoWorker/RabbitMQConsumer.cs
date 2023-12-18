using System.Text;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ToDoWorker
{
    public class RabbitMQConsumer
    {
        public RabbitMQConsumer()
        {
        }

        public void StartListening()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare a named direct exchange
            channel.ExchangeDeclare(exchange: "my_direct_exchange", type: ExchangeType.Direct);

            // Declare the queue
            channel.QueueDeclare(queue: "todo_completed",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Bind the queue to the exchange with a routing key
            channel.QueueBind(queue: "todo_completed",
                              exchange: "my_direct_exchange",
                              routingKey: "");

            // Create a consumer
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                Console.WriteLine($"Received message: {message}");

                // Acknowledge the message
                channel.BasicAck(ea.DeliveryTag, false);
            };

            // Start consuming messages
            channel.BasicConsume(queue: "todo_completed", autoAck: false, consumer: consumer);

            Console.WriteLine("Subscriber listening for messages. Press any key to exit.");
            Console.ReadLine();
        }
    }
}