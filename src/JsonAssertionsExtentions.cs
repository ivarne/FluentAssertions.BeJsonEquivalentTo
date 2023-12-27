using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;

namespace FluentAssertions.JsonEquivalent;
using FluentAssertions.Primitives;

public static class FluentAssertionsExtentions
{
    public static AndConstraint<StringAssertions> BeJsonEquivalentTo(this StringAssertions actual, string expected, JsonComparatorOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(actual, "Cannot assert string containment against <null>.");

        string? result;
        if (options?.LooseObjectOrderComparison != true)
        {
            result = JsonTokenStreamCompare.IsJsonTokenEquivalent(actual.Subject, expected, options);
        }
        else
        {
            result = JsonLooseOrderComparison.IsJsonTokenEquivalent(actual.Subject, expected, options);
        }

        if (result is not null)
        {
            Execute.Assertion
                .FailWith(() => new FailReason("Expected {context:string} to match expected json, but found diff\n{0}", result));
        }

        return new AndConstraint<StringAssertions>(actual);
    }
}