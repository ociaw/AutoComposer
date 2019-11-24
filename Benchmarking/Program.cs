using System;
using BenchmarkDotNet.Running;

namespace Benchmarking
{
    public class Program
    {
        public static void Main()
        {
            _ = BenchmarkRunner.Run<OrderedComposition>();
        }
    }
}