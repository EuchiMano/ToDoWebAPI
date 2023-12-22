using System.Text;
using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SkiaSharp;

namespace ToDoWorker
{
    public class RabbitMQConsumer
    {
        private readonly BadgeGenerator _badgeGenerator;
        private readonly RabbitMqPublisher _rabbitMqPublisher;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RabbitMQConsumer(BadgeGenerator badgeGenerator,
        RabbitMqPublisher rabbitMqPublisher,
        ILogger<RabbitMQConsumer> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
        {
            _badgeGenerator = badgeGenerator;
            _rabbitMqPublisher = rabbitMqPublisher;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public void StartListening()
        {
            try
            {
                var factory = CreateConnectionFactory();
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                DeclareExchangeAndQueue(channel);

                var consumer = CreateMessageConsumer(channel);
                consumer.Received += (model, ea) => ProcessMessage(ea, channel);

                // Start consuming messages
                channel.BasicConsume(queue: "todo_completed", autoAck: false, consumer: consumer);

                _logger.LogInformation("Subscriber listening for messages. Press any key to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RabbitMQConsumer: {ex.Message}");
            }
        }

        private ConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = "localhost", // Update with your RabbitMQ server information
                UserName = "guest",
                Password = "guest"
            };
        }

        private void DeclareExchangeAndQueue(IModel channel)
        {
            channel.ExchangeDeclare(exchange: "my_direct_exchange", type: ExchangeType.Direct);

            channel.QueueDeclare(queue: "todo_completed", durable: false, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind(queue: "todo_completed", exchange: "my_direct_exchange", routingKey: "");
        }

        private EventingBasicConsumer CreateMessageConsumer(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);
            return consumer;
        }

        private async Task ProcessMessage(BasicDeliverEventArgs ea, IModel channel)
        {
            try
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                // Deserialize the JSON payload into your model
                var toDoCompletedEvent = JsonConvert.DeserializeObject<TodoCompletedMessage>(message);

                Console.WriteLine($"Received message: ToDoCompletedEvent Id={toDoCompletedEvent.TodoId}");

                // Perform badge generation based on the received message
                string badgePath = _badgeGenerator.GenerateBadge("Curious", SKColors.Blue);

                // Save the badge image to S3
                string s3Url = await SaveToS3Async(badgePath);

                // Call the Web API to update the Todo with the badge URL
                await UpdateTodoBadgeUrlAsync(toDoCompletedEvent.TodoId, badgePath);

                // Publish the badge path
                //_rabbitMqPublisher.PublishMessage("badge_queue", badgePath);

                // Acknowledge the message
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
                // You might want to handle the error in a way that suits your application
            }
        }

        private async Task UpdateTodoBadgeUrlAsync(Guid todoId, string badgePath)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiUrl = "http://localhost:5156/api/Todo/update-badge"; // Replace with your Web API URL

            var content = new StringContent(JsonConvert.SerializeObject(new { TodoId = todoId, BadgeUrl = badgePath }), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Todo updated successfully.");
            }
            else
            {
                _logger.LogError($"Failed to update Todo. Status code: {response.StatusCode}");
                // You might want to handle the failure in a way that suits your application
            }
        }

        private async Task<string> SaveToS3Async(string localFilePath)
        {
            try
            {
                var awsAccessKey = _configuration["AWS:AccessKey"];
                var awsSecretKey = _configuration["AWS:SecretKey"];
                var awsRegion = _configuration["AWS:Region"];
                var awsBucketName = _configuration["AWS:BucketName"];

                using var s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.GetBySystemName(awsRegion));

                var key = $"badge/{Guid.NewGuid()}.png"; // Change the key format as needed

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    FilePath = localFilePath,
                    BucketName = awsBucketName,
                    Key = key,
                };

                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);

                // Construct and return the S3 URL
                var s3Url = $"https://{awsBucketName}.s3.amazonaws.com/{key}";
                return s3Url;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving to S3: {ex.Message}");
                throw; // You might want to handle the error in a way that suits your application
            }
        }
    }
}