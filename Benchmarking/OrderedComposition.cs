using System;
using BenchmarkDotNet.Attributes;
using AutoComposer;

namespace Benchmarking
{
    public class OrderedComposition
    {
        [Benchmark]
        public Middle SingleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Middle(), new Bottom(), new Bottom() };
            Middle middle = composer.Compose<Middle>(objects);
            return middle;
        }

        [Benchmark]
        public Top DoubleNest()
        {
            Composer composer = new Composer();
            Object[] objects = { new Top(), new Middle(), new Bottom(), new Bottom(), new Middle(), new Bottom(), new Bottom() };
            Top top = composer.Compose<Top>(objects);
            return top;
        }
        
        [Benchmark]
        public (Top, Top) DoubleNestTwice()
        {
            return (DoubleNest(), DoubleNest());
        }

        public class Top
        {
            [Composable]
            [AssignOrder(2)]
            public Middle Middle2 { get; set; }

            [Composable]
            [AssignOrder(1)]
            public Middle Middle { get; set; }
        }

        public class Middle
        {
            [Composable]
            [AssignOrder(2)]
            public Bottom Bottom2 { get; set; }

            [Composable]
            [AssignOrder(1)]
            public Bottom Bottom { get; set; }
        }

        public class Bottom
        {

        }
    }
}
