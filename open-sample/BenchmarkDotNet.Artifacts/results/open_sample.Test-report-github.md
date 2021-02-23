``` ini

BenchmarkDotNet=v0.12.1, OS=macOS Catalina 10.15.7 (19H2) [Darwin 19.6.0]
Intel Core i7-8850H CPU 2.60GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT


```
|                     Method |     Mean |    Error |   StdDev |  Median |      P95 |   Op/s |     Gen 0 |     Gen 1 | Gen 2 | Allocated |
|--------------------------- |---------:|---------:|---------:|--------:|---------:|-------:|----------:|----------:|------:|----------:|
| With_OpenChannelExtensions | 10.894 s | 3.3511 s | 9.7223 s | 4.454 s | 30.048 s | 0.0918 | 4000.0000 | 1000.0000 |     - |  17.09 MB |
|              With_Channels |  2.588 s | 0.2119 s | 0.5618 s | 2.489 s |  3.392 s | 0.3863 | 5000.0000 | 2000.0000 |     - |  26.97 MB |
