﻿using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Hosting;
using VoyageIntoDeadlocking.Grains;

namespace VoyageIntoDeadlocking
{
    internal static class Program
    {
       
        private static async Task Main(string[] args)
        {
            var host = await StartSilo();
            var client = await StartClient();

            await client.GetGrain<IAlienPlanet>(Guid.NewGuid()).Create();
            await client.GetGrain<IAlienPlanet>(Guid.NewGuid()).Create();
            await client.GetGrain<IAlienPlanet>(Guid.NewGuid()).Create();
            await client.GetGrain<IAlienPlanet>(Guid.NewGuid()).Create();
            await client.GetGrain<IAlienPlanet>(Guid.NewGuid()).Create();
            await client.GetGrain<IAlienPlanet>(Guid.NewGuid()).Create();
            
            
            var planetEarthId = Guid.Empty;
            var earthGrain = client.GetGrain<IRadioControl>(planetEarthId);
            await earthGrain.BroadcastMessage("Onwards into the void!!");

            Console.WriteLine("Press key to exit...");
            Console.ReadKey();

            Console.WriteLine("Stopping server...");
            await host.StopAsync();
        }

        private static async Task<IClusterClient> StartClient()
        {
            var client = new ClientBuilder()
                .AddSimpleMessageStreamProvider(Streams.RadioStreamName)
                .UseLocalhostClustering()
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }


        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .AddSimpleMessageStreamProvider(Streams.RadioStreamName)
                .AddMemoryGrainStorageAsDefault()
                .UseLocalhostClustering();

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}