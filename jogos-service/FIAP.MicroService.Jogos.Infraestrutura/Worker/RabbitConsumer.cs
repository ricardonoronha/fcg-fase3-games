using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace FIAP.MicroService.Jogos.Infraestrutura.Worker
{
    public class RabbitConsumer : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitConsumer(ConnectionFactory factory) => _factory = factory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(
                exchange: "user_exchange",
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: "jogos_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: "jogos_queue",
                exchange: "user_exchange",
                routingKey: "",
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[JOGOS] - Mensagem recebida: {message}");

                    // TODO: processar de verdade aqui

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[JOGOS] - Erro processando mensagem: {ex}");

                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "jogos_queue",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            // Mantém o serviço vivo até cancelar
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try { if (_channel is not null) await _channel.CloseAsync(cancellationToken); } catch { }
            await base.StopAsync(cancellationToken);
        }
    }
}