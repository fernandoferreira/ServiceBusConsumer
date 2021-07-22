using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBusConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Testando consume de mensagens do AZ Service Bus!!!");

            if (args.Length != 2)
            {
                System.Console.WriteLine(@"Você deve informar 2 argumentos: 
                String de conexão do Azure Service Bus e 
                a Fila (nome) a ser utilizada.");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<Config>(
                    new Config()
                    {
                        ConnectionString = args[0],
                        Queue = args[1]
                    });
                services.AddHostedService<Worker>();    
            });
    

    }
}
