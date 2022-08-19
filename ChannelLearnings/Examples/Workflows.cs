using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChannelLearnings.Examples
{
    public class Workflows
    {
        public async Task Run()
        {
            var step1Channel = Channel.CreateUnbounded<string>();
            var step2Channel = Channel.CreateUnbounded<string>();
            var step3Channel = Channel.CreateUnbounded<string>();

            var finalChannel = Channel.CreateUnbounded<string>();
            var startingLetters = new[] {"B", "K", "P", "R"};

            var workflowStep1 = new WorkflowStep1(step1Channel.Reader, step2Channel.Writer);
            var workflowStep2 = new WorkflowStep2(step2Channel.Reader, step3Channel.Writer);
            var workflowStep3 = new WorkflowStep3(step3Channel.Reader, finalChannel.Writer);


            Task.Run(workflowStep1.Run);
            Task.Run(workflowStep2.Run);
            Task.Run(workflowStep3.Run);

            foreach (var item in startingLetters)
            {
                await step1Channel.Writer.WriteAsync(item);
            }

            // uncommenting this will complete the channel, which will pop off all the 
            // other channel completes under the IAsyncEnumerables
            //step1Channel.Writer.Complete();

            await foreach (string item in finalChannel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Final word: {item}");               
            }

            Console.WriteLine("All channels completions cascaded");
        }
    }


    public class WorkflowStep1
    {
        private readonly ChannelReader<string> _channelReader;
        private readonly ChannelWriter<string> _channelWriter;
        public WorkflowStep1(ChannelReader<string> channelReader, ChannelWriter<string> channelWriter)
        {
            _channelReader = channelReader;
            _channelWriter = channelWriter;
        }

        public async Task Run()
        {
            await foreach (string item in _channelReader.ReadAllAsync())
            {
                Console.WriteLine($"In {GetType().Name}, processing {item}");
                await _channelWriter.WriteAsync($"{item}i");
            }

            _channelWriter.Complete();
        }
    }

    public class WorkflowStep2
    {
        private readonly ChannelReader<string> _channelReader;
        private readonly ChannelWriter<string> _channelWriter;
        public WorkflowStep2(ChannelReader<string> channelReader, ChannelWriter<string> channelWriter)
        {
            _channelReader = channelReader;
            _channelWriter = channelWriter;
        }

        public async Task Run()
        {
            await foreach (string item in _channelReader.ReadAllAsync())
            {
                Console.WriteLine($"In {GetType().Name}, processing {item}");
                await _channelWriter.WriteAsync($"{item}n");
            }
            _channelWriter.Complete();

        }
    }

    public class WorkflowStep3
    {
        private readonly ChannelReader<string> _channelReader;
        private readonly ChannelWriter<string> _channelWriter;
        public WorkflowStep3(ChannelReader<string> channelReader, ChannelWriter<string> channelWriter)
        {
            _channelReader = channelReader;
            _channelWriter = channelWriter;
        }

        public async Task Run()
        {
            await foreach (string item in _channelReader.ReadAllAsync())
            {
                Console.WriteLine($"In {GetType().Name}, processing {item}");
                await _channelWriter.WriteAsync($"{item}g");
            }
            _channelWriter.Complete();

        }
    }
}
