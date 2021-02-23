``` ini

BenchmarkDotNet=v0.12.1, OS=macOS Catalina 10.15.7 (19H2) [Darwin 19.6.0]
Intel Core i7-8850H CPU 2.60GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT


```
|                    Method |        Mean |     Error |    StdDev |      Gen 0 | Gen 1 | Gen 2 |   Allocated |
|-------------------------- |------------:|----------:|----------:|-----------:|------:|------:|------------:|
|     Channel_ReadThenWrite |    95.18 ms |  0.368 ms |  0.344 ms |          - |     - |     - |       155 B |
| BufferBlock_ReadThenWrite | 1,221.26 ms | 19.142 ms | 17.905 ms | 49000.0000 |     - |     - | 232002304 B |
