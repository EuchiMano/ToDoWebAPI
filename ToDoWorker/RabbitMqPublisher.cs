using System.Text;
using RabbitMQ.Client;

namespace ToDoWorker
{
    public class RabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;
        private readonly string _exchangeName;

        public RabbitMqPublisher(string hostName, string exchangeName)
        {
            _factory = new ConnectionFactory() { HostName = hostName };
            _exchangeName = exchangeName;
        }

        public void PublishMessage(string queueName, string message)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: _exchangeName, routingKey: queueName, basicProperties: null, body: body);
            }
        }
    }
}