using System;
using BenchmarkDotNet.Running;

namespace Sample01
{
    internal static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<Test>();
        }
    }
}