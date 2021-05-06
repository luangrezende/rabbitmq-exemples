using Mail;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace Customer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "qSendMail",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonSerializer.Serialize(GenerateMessage());

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "qSendMail",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine("[x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public static MailRequest GenerateMessage()
        {
            return new MailRequest
            {
                Mail = "luanteste2@mailinator.com",
                MailType = MailTypeEnum.UserCreated
            };
        }
    }
}
