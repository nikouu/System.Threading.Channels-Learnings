using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelLearnings.Examples
{
    public class Basic
    {
        public async Task Run()
        {
            var channel = Channel.CreateUnbounded<string>();

            for (int i = 0; i < 10; i++)
            {
                await channel.Writer.WriteAsync($"Value: {i} at {DateTime.Now:O}");
            }

            while (true)
            {
                var item = await channel.Reader.ReadAsync();
                Console.WriteLine(item);
            }
        }

        public async Task Run2()
        {
            var channel = Channel.CreateUnbounded<string>();

            for (int i = 0; i < 10; i++)
            {
                await channel.Writer.WriteAsync($"Value: {i} at {DateTime.Now:O}");
            }

            while (await channel.Reader.WaitToReadAsync())            
                while(channel.Reader.TryRead(out string item))
                    Console.WriteLine(item);            
        }

        public async Task Run3()
        {
            var channel = Channel.CreateUnbounded<string>();

            for (int i = 0; i < 10; i++)
            {
                await channel.Writer.WriteAsync($"Value: {i} at {DateTime.Now:O}");
            }

            // takes advantage of the IAsyncEnumerable
            await foreach(string item in channel.Reader.ReadAllAsync())
                Console.WriteLine(item);
        }
    }
}
