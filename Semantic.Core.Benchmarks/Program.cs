using BenchmarkDotNet.Running;

namespace Semantic.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<CacheBenchmarks>();
    }
}