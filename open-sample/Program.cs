using System.Threading.Tasks;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using open_sample.License;
namespace open_sample
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            //BenchmarkRunner.Run<Test>();
            //run as console
            var withChannelExtensions = new WithChannelExtensions();
            await withChannelExtensions.RunWithChannelsAsync();

            var withChannels = new WithChannels();
            await withChannels.RunWithChannelsAsync();

            var withoutChannels = new WithoutChannels();
            await withoutChannels.RunWithoutChannelsAsync();

//          Console output
//          With Open Extensions ==> Completed in 00:00:03.8356042
//          With Channel ==> Completed in 00:00:02.9015385
//          Without Channels ==> Completed in 00:00:29.8277077
        }
    }
    [MemoryDiagnoser]
    [Config(typeof(Config))]
    public class Test
    {
        [Benchmark]
        public async Task With_OpenChannelExtensions()
        {
            var withChannelExtensions = new WithChannelExtensions();
            await withChannelExtensions.RunWithChannelsAsync(); 
        }
        [Benchmark]
        public async Task With_Channels()
        {
            var withChannels = new WithChannels();
            await withChannels.RunWithChannelsAsync();
        }
        [Benchmark]
        public async Task Without_Channels()
        {
            var withoutChannels = new WithoutChannels();
            await withoutChannels.RunWithoutChannelsAsync();
        }
    }

    public class Config : ManualConfig
    {
        public Config()
        {
            AddColumn(StatisticColumn.OperationsPerSecond);
            AddColumn(StatisticColumn.P95);
            AddColumn(StatisticColumn.Median);
            AddAnalyser(new BaselineCustomAnalyzer());
        }
    }
}