using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;

namespace JsonCompare;
using FluentAssertions.Primitives;

public static class JsonAssertionsExtentions
{
    public static AndConstraint<StringAssertions> BeJsonTokenEquivalentTo(this StringAssertions actual, string expected,
        JsonComparatorOptions? options = null)
    {
        var result = JsonCompareToken.IsJsonTokenEquivalent(actual.Subject, expected, options);

        if (result is not null)
        {
            Execute.Assertion.FailWith("{0}\n{1}\n{2}\n{3}", result.message, result.expected, result.actual, result.posMark);
        }


        return new AndConstraint<StringAssertions>(actual);
    }
    public static AndConstraint<StringAssertions> BeJsonEquivalentTo(this StringAssertions actual, string expected, JsonComparatorOptions? options = null)
    {
       
        var result = JsonCompare.IsJsonEquivalent(actual.Subject, expected, options);
        
        if (result is not null)
        {
            Execute.Assertion.FailWith("{0}\n{1}\n{2}\n{3}", result.message, result.expected, result.actual, result.posMark);
        }
        
        return new AndConstraint<StringAssertions>(actual);
    }
}
