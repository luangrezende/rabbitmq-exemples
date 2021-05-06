using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace Mail
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            _ = channel.QueueDeclare(queue: "qSendMail",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<MailRequest>(Encoding.UTF8.GetString(body));

                SendMail(message);

                Console.WriteLine("[x] Received {0}", message);
            };

            channel.BasicConsume(queue: "qSendMail",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }

        public static void SendMail(MailRequest mailRequest)
        {
            var fromAddress = new MailAddress("luangomesrezende@gmail.com", "Luan Mail");
            var toAddress = new MailAddress(mailRequest.Mail, "Client");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "gomesluan1369")
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "testeee",
                Body = "testando envio"
            })
            {
                smtp.Send(message);
            }
        }
    }
}
