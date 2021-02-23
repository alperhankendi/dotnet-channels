using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace Sample01
{
    [MemoryDiagnoser]
    [Config(typeof(Config))]
    public class Test
    {
        private readonly Channel<int> c = Channel.CreateBounded<int>(new BoundedChannelOptions(2)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait,
        });

        private readonly Channel<int> _channel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions());
        [Benchmark]
        public async Task Channel_ReadThenWrite()
        {
            ChannelWriter<int> writer = _channel.Writer;
            ChannelReader<int> reader = _channel.Reader;
            for (int i = 0; i < 1_000_000; i++)
            {
                ValueTask<int> vt = reader.ReadAsync();
                writer.TryWrite(i);
                await vt;
            }
        }
        private readonly MyChannel<int> _myChannel = new MyChannel<int>();
        [Benchmark]
        public async Task MyChannel_ReadThenWrite()
        {
            for (int i = 0; i < 1_000_000; i++)
            {
                ValueTask<int> vt = _myChannel.ReadAsync();
                _myChannel.Write(i);
                await vt;
            }
        }
        private class Config : ManualConfig
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
}