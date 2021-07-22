using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceBusConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Config _configuracoes;
        private readonly QueueClient _client;


        public Worker(ILogger<Worker> logger, Config configuracoes)
        {
            logger.LogInformation($"Fila = {configuracoes.Queue}");

            _logger = logger;
            _configuracoes = configuracoes;
            _client = new(
                _configuracoes.ConnectionString,
                _configuracoes.Queue,
                ReceiveMode.ReceiveAndDelete);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Processando mensagem...");

                    _client.RegisterMessageHandler(async (message, stoppingToken) =>
                    {
                        await ProcessarMensagem(message);
                    },
                    new MessageHandlerOptions(
                        async (e) =>
                        {
                            await ProcessarErro(e);
                        }
                    )
                    );
                }
            });
        }

        private Task ProcessarErro(ExceptionReceivedEventArgs e)
        {
            _logger.LogError("Falha: " +
                e.Exception.GetType().FullName + " " +
                e.Exception.Message);
            return Task.CompletedTask;
        }

        private Task ProcessarMensagem(Message message)
        {
            var conteudo = Encoding.UTF8.GetString(message.Body);
            _logger.LogInformation($"Mensagem recebida: {conteudo}");
            return Task.CompletedTask;
        }
    }
}