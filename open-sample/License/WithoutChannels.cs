using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace open_sample.License
{
    public class WithoutChannels
    {
        private readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        public async Task RunWithoutChannelsAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using var reader = new StreamReader(File.OpenRead(LicenseStrings.InputFilePath));
            await using var writer = new StreamWriter(File.OpenWrite("../../../withoutchannels-output.txt"));
            string? url;
            
            while ((url = await reader.ReadLineAsync()) != null)
            {
                url = url.Trim();
                
                var contents = await _httpClient.GetStringOrNullAsync(url);
                
                var licenseIdentifier = LicenseStrings.TryIdentify(contents ?? string.Empty);

                await writer.WriteLineAsync($"{url};{licenseIdentifier}");
            }
            
            
            stopwatch.Stop();
            Console.WriteLine("Without Channels ==> Completed in {0}", stopwatch.Elapsed);
        }
    }
}