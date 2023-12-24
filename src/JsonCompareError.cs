namespace FluentAssertions.JsonEquivalent;

public record JsonCompareError(string message, string actual, string expected, string posMark);