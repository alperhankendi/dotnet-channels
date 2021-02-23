using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace open_sample.License
{
    public class WithChannels
    {
        private readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        private async Task Wrap(Func<Task> action)
        {
            await action.Invoke();
        }
       
        public async Task RunWithChannelsAsync()
        {
            var downloadUriChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(50000) { SingleReader = false, SingleWriter = true });
            var processContentChannel = Channel.CreateBounded<(string, string?)>(new BoundedChannelOptions(100) { SingleReader = false, SingleWriter = false });
            var writeOutputChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(50000) { SingleReader = true, SingleWriter = false });

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tasks = new List<Task>();
            
            // 1. Read file
            tasks.Add(Wrap(async () =>
            {
                using var reader = new StreamReader(File.OpenRead(LicenseStrings.InputFilePath));

                string? url;
                while ((url = await reader.ReadLineAsync()) != null)
                {
                    await downloadUriChannel.Writer.WriteAsync(url.Trim());
                } 

                downloadUriChannel.Writer.TryComplete();
            }));
            
            // 2. Download license
            var downloadTasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                downloadTasks.Add(Wrap(async () =>
                {
                    while (await downloadUriChannel.Reader.WaitToReadAsync())
                    {
                        while (downloadUriChannel.Reader.TryRead(out var url))
                        {
                            var contents = await _httpClient.GetStringOrNullAsync(url);
                            await processContentChannel.Writer.WriteAsync((url, contents));
                        }
                    }
                }));
            }
            tasks.Add(Wrap(async () =>
            {
                await Task.WhenAll(downloadTasks);
                processContentChannel.Writer.TryComplete();
            }));
            
            // 3. Check for words
            var processTasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                processTasks.Add(Wrap(async () =>
                {
                    while (await processContentChannel.Reader.WaitToReadAsync())
                    {
                        while (processContentChannel.Reader.TryRead(out var tuple))
                        {
                            var licenseIdentifier = LicenseStrings.TryIdentify(tuple.Item2 ?? string.Empty);
                            await writeOutputChannel.Writer.WriteAsync($"{tuple.Item1};{licenseIdentifier}");
                        }
                    }
                }));
            }
            tasks.Add(Wrap(async () =>
            {
                await Task.WhenAll(processTasks);
                writeOutputChannel.Writer.TryComplete();
            }));
            
            // 4. Write to output file
            tasks.Add(Wrap(async () =>
            {
                await using var writer = new StreamWriter(File.OpenWrite("../../../withchannels-output.txt"));

                while (await writeOutputChannel.Reader.WaitToReadAsync())
                {
                    while (writeOutputChannel.Reader.TryRead(out var line))
                    {
                        await writer.WriteLineAsync(line);
                    }
                }
            }));

            await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            Console.WriteLine("With Channel ==> Completed in {0}", stopwatch.Elapsed);
        }
    }
}