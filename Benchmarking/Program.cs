using System;
using BenchmarkDotNet.Running;

namespace Benchmarking
{
    public class Program
    {
        public static void Main(String[] args)
        {
            var summary = BenchmarkRunner.Run<OrderedComposition>();
        }
    }
}