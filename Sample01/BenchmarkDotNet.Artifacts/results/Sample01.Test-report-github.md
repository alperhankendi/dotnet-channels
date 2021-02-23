``` ini

BenchmarkDotNet=v0.12.1, OS=macOS Catalina 10.15.7 (19H2) [Darwin 19.6.0]
Intel Core i7-8850H CPU 2.60GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT


```
|                  Method |       Mean |    Error |    StdDev |     Median |        P95 |   Op/s |      Gen 0 | Gen 1 | Gen 2 |    Allocated |
|------------------------ |-----------:|---------:|----------:|-----------:|-----------:|-------:|-----------:|------:|------:|-------------:|
|   Channel_ReadThenWrite |   126.3 ms |  1.45 ms |   1.13 ms |   126.1 ms |   128.0 ms | 7.9188 |          - |     - |     - |      2.24 KB |
| MyChannel_ReadThenWrite | 1,832.6 ms | 55.12 ms | 159.92 ms | 1,831.0 ms | 2,136.6 ms | 0.5457 | 50000.0000 |     - |     - | 226563.02 KB |
