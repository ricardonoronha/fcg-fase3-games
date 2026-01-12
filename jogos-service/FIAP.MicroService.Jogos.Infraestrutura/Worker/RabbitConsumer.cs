using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace FIAP.MicroService.Jogos.Infraestrutura.Worker
{
    public class RabbitConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private IChannel? _channel;

        public RabbitConsumer(IConnection connection)
        {
            this._connection = connection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("user_exchange", ExchangeType.Fanout);
            await _channel.QueueDeclareAsync(queue: "jogos_queue", durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(queue: "jogos_queue", exchange: "user_exchange", routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[JOGOS] - Mensagem recebida: {message}");

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: "jogos_queue", autoAck: true, consumer: consumer);
        }
    }
}