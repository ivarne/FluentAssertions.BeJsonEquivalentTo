using System.Xml.XPath;
using FluentAssertions.JsonEquivalent.Tests.Utils;
using Xunit.Abstractions;

namespace FluentAssertions.JsonEquivalent.Tests;

public class FuzzerTests
{
    private readonly ITestOutputHelper _output;

    public FuzzerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(1002)]
    [InlineData(1003)]
    [InlineData(1004)]
    [InlineData(1005)]
    [InlineData(1006)]
    [InlineData(1007)]
    public void TestFuzzed(int seed)
    {
        var escapeActual = (new Random(seed)).Next() % 2 == 0;
        var actual = JsonFuzzer.GetFuzzedJson(seed, new()
        {
            Escape = escapeActual,
        });
        _output.WriteLine(actual);
        var expected = JsonFuzzer.GetFuzzedJson(seed, new JsonFuzzer.JsonFuzzerOptions()
        {
            Escape = !escapeActual,
        });
        JsonTokenStreamCompare.IsJsonTokenEquivalent(actual, expected, new JsonComparatorOptions()).Should().BeNull();

        JsonLooseOrderComparison.IsJsonTokenEquivalent(actual, actual, new JsonComparatorOptions()
        {
            LooseObjectOrderComparison = false,
        }).Should().BeNull();
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(1002)]
    [InlineData(1003)]
    [InlineData(1004)]
    [InlineData(1005)]
    [InlineData(1006)]
    [InlineData(1007)]
    public void TestFuzzedRandomOrder(int seed)
    {
        var escapeActual = (new Random(seed)).Next() % 2 == 0;
        var actual = JsonFuzzer.GetFuzzedJson(seed, new JsonFuzzer.JsonFuzzerOptions()
        {
            Escape = escapeActual,
            // Randomization is deterministic based on seed, so we need to compare randomized with not randomized
            RandomizePropertyOrder = false,
        });
        _output.WriteLine(actual);
        var expected = JsonFuzzer.GetFuzzedJson(seed, new JsonFuzzer.JsonFuzzerOptions()
        {
            Escape = !escapeActual,
            RandomizePropertyOrder = true,
        });

        JsonLooseOrderComparison.IsJsonTokenEquivalent(actual, expected, new JsonComparatorOptions()
        {
            LooseObjectOrderComparison = true,
        }).Should().BeNull();
    }

    [Fact(Skip = "Takes way too long")]
    public void TestLotsOfConditions()
    {
        foreach (var i in Enumerable.Range(0, 100000))
        {
            TestFuzzed(i);
            TestFuzzedRandomOrder(i);
        }
    }
}