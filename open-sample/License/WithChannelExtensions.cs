using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using Open.ChannelExtensions;
//https://open-net-libraries.github.io/Open.ChannelExtensions/api/Open.ChannelExtensions.html
namespace open_sample.License
{
    public class WithChannelExtensions
    {
        private readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        private IEnumerable<string> ReadInput()
        {
            using var reader = new StreamReader(File.OpenRead(LicenseStrings.InputFilePath));
            string? url;
            while ((url = reader.ReadLine()) != null)
            {
                yield return url.Trim();
            }
        }
        public async Task RunWithChannelsAsync()
        {
            // 1. Read file
            // 2. Download license
            // 3. Check content for known words
            // 4. Write to other file
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            await using var writer = new StreamWriter(File.OpenWrite("../../../ext-output.txt"));
            
            await Channel
                .CreateBounded<string>(50000)
                .Source(ReadInput())
                .PipeAsync(
                    maxConcurrency: 100,
                    capacity: 500,
                    transform: async url =>
                    {
                        var contents = await _httpClient.GetStringOrNullAsync(url);
                        return (url, contents);
                    })
                .Pipe(
                    maxConcurrency: 10,
                    capacity: 5000,
                    transform: tuple =>
                    {
                        var licenseIdentifier = LicenseStrings.TryIdentify(tuple.Item2 ?? string.Empty);
                        return $"{tuple.Item1};{licenseIdentifier}";
                    })
                .ReadAllAsync(async line =>
                {
                    await writer.WriteLineAsync(line);
                });
            
            stopwatch.Stop();
            Console.WriteLine("With Open Extensions ==> Completed in {0}", stopwatch.Elapsed);
        }
    }
}