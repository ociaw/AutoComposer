using System;
using BenchmarkDotNet.Attributes;
using AutoComposer;

namespace Benchmarking
{
    public class OrderedComposition
    {
        private readonly Composer _composer = new Composer();

        [Benchmark]
        public Middle SingleNest()
        {
            Object[] objects = { new Middle(), new Bottom(), new Bottom() };
            Middle middle = _composer.Compose<Middle>(objects);
            return middle;
        }

        [Benchmark]
        public Top DoubleNest()
        {
            Object[] objects = { new Top(), new Middle(), new Bottom(), new Bottom(), new Middle(), new Bottom(), new Bottom() };
            Top top = _composer.Compose<Top>(objects);
            return top;
        }
        
        [Benchmark]
        public Type[] Flatten()
        {
            Type[] types = _composer.FlattenComposableType<Top>();
            return types;
        }

        public class Top
        {
            [Composable(2)]
            public Middle Middle2 { get; set; }

            [Composable(1)]
            public Middle Middle { get; set; }
        }

        public class Middle
        {
            [Composable(2)]
            public Bottom Bottom2 { get; set; }

            [Composable(1)]
            public Bottom Bottom { get; set; }
        }

        public class Bottom
        {

        }
    }
}
