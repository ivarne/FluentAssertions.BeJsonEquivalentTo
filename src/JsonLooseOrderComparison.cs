using System.Diagnostics;
using System.Text.Json;

namespace FluentAssertions.JsonEquivalent;

internal static class JsonLooseOrderComparison
{
    internal static string? IsJsonTokenEquivalent(string actual, string expected, JsonComparatorOptions? options = null)
    {
        var actualBytes = System.Text.Encoding.UTF8.GetBytes(actual);
        var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
        return IsJsonTokenEquivalent(actualBytes, expectedBytes, options);
    }

    internal static string? IsJsonTokenEquivalent(byte[] actualBytes, byte[] expectedBytes, JsonComparatorOptions? options = null)
    {
        options ??= JsonCompareUtils.DefaultOptions;

        var state = new ComparatorState(actualBytes, expectedBytes, options.Value);
        var comparisonResult = CompareReaders(ref state);
        return comparisonResult;
    }

    private static string? CompareReaders(ref ComparatorState state)
    {
        Debug.Assert(state.Actual.TokenType == state.Expected.TokenType);
        while (state.Actual.Read() && state.Expected.Read())
        {
            var tokenError = CompareToken(ref state);
            if (tokenError is not null) return tokenError;
        }

        if (state.Actual.Read())
        {
            return JsonCompareUtils.CreateErrorMessage("Actual has more tokens than expected", ref state);
        }

        if (state.Expected.Read())
        {
            return JsonCompareUtils.CreateErrorMessage("Expected has more tokens than actual", ref state);
        }

        return null;
    }

    private static string? CompareToken(ref ComparatorState state)
    {
        if (state.Actual.TokenType != state.Expected.TokenType)
        {
            return JsonCompareUtils.CreateErrorMessage("JsonTokenType mismatch", ref state);
        }

        switch (state.Actual.TokenType)
        {
            // if we use loose object order comparison we need to use the special function to compare objects
            case JsonTokenType.StartObject:
                string? error = CompareObject(ref state);
                if (error is not null) return error;

                break;
            case JsonTokenType.StartArray:
                while (state.Actual.Read() &&
                       state.Expected.Read() &&
                       state.Actual.TokenType != JsonTokenType.EndArray &&
                       state.Expected.TokenType != JsonTokenType.EndArray)
                {

                    var tokenError = CompareToken(ref state);
                    if (tokenError is not null) return tokenError;
                }

                break;
            // Ignore tokens without value
            case JsonTokenType.EndObject:
            case JsonTokenType.EndArray:
            case JsonTokenType.None:
            case JsonTokenType.True:
            case JsonTokenType.False:
            case JsonTokenType.Null:
                break;

            // compare the value of tokens with value
            case JsonTokenType.PropertyName:
                if (!state.Actual.ValueTextEquals(state.Expected.ValueSpan))
                {
                    return JsonCompareUtils.CreateErrorMessage("PropertyName mismatch (validate strict order)", ref state);
                }

                break;
            case JsonTokenType.String:
                if (!state.Actual.ValueTextEquals(state.Expected.ValueSpan))
                {
                    return JsonCompareUtils.CreateErrorMessage("string mismatch", ref state);
                }

                break;
            case JsonTokenType.Comment: // compare comments if they are not ignored
                if (state.Actual.GetComment() != state.Expected.GetComment())
                {
                    return JsonCompareUtils.CreateErrorMessage("comment mismatch", ref state);
                }

                break;
            case JsonTokenType.Number:
                if (state.Actual.GetDecimal() != state.Expected.GetDecimal())
                {
                    return JsonCompareUtils.CreateErrorMessage("number mismatch", ref state);
                }

                break;
        }

        return null;
    }

    private static string? CompareObject(ref ComparatorState state)
    {
        Debug.Assert(state.Actual.TokenType == JsonTokenType.StartObject);
        Debug.Assert(state.Expected.TokenType == JsonTokenType.StartObject);

        var expectedJsonCache = state.Options.LooseObjectOrderComparison? new Dictionary<string, int>(): null;

        state.Actual.Read();
        state.Expected.Read();
        while (state.Actual.TokenType == JsonTokenType.PropertyName &&
               state.Expected.TokenType == JsonTokenType.PropertyName)
        {
            // Simple compare if properties are equal
            if (state.Expected.ValueTextEquals(state.Actual.ValueSpan))
            {
                var error = CompareProperty(ref state);
                if (error is not null) return error;
                // ensure that expected is read to the next property
                state.Expected.Read();
                state.Actual.Read();
            }
            // If we use loose object order comparison we need to cache the expected properties
            else if (expectedJsonCache is not null)
            {
                // If we find the expected property name in the actual json cache
                // create a new state with a rewind actual reader
                if (expectedJsonCache.Remove(state.Expected.GetString()!, out var expectedOffset))
                {
                    var newState = new ComparatorState(ref state, expectedOffset);
                    var error = CompareToken(ref newState);
                    if (error is not null) return error;
                    state.Expected.Read();
                    state.Actual.Read();
                }
                else
                {
                    // Read a full property from the expected json and cache it so that we can compare with actual later
                    var expectedPropertyName = state.Expected.GetString();
                    state.Expected.Read();
                    expectedJsonCache.Add(expectedPropertyName!, state.ExpectedOffset);
                    ReadFullProperty(ref state.Expected);
                }
            }
            else
            {
                return JsonCompareUtils.CreateErrorMessage("PropertyName mismatch (validate strict order)", ref state);
            }
        }
        if (state.Actual.TokenType == JsonTokenType.PropertyName && expectedJsonCache is not null)
        {
            Debug.Assert(state.Expected.TokenType == JsonTokenType.EndObject);
            while (state.Actual.TokenType == JsonTokenType.PropertyName)
            {
                if (expectedJsonCache.Remove(state.Actual.GetString()!, out var expectedOffset))
                {
                    state.Actual.Read();
                    var newState = new ComparatorState(ref state, expectedOffset);
                    var error = CompareToken(ref newState);
                    if (error is not null) return error;
                    // synchronize the actual reader with the new state
                    while(newState.Actual.TokenStartIndex > state.Actual.TokenStartIndex)
                        state.Actual.Read();
                    // read the next token
                    state.Actual.Read();
                }
                else
                {
                    return JsonCompareUtils.CreateErrorMessage($"Expected property {state.Actual.GetString()} not found in actual", ref state);
                }
            }
        }

        if (expectedJsonCache?.Count > 0)
        {
            return JsonCompareUtils.CreateErrorMessage($"Expected properties {string.Join(",", expectedJsonCache.Keys)} not found in actual", ref state);
        }

        Debug.Assert(state.Actual.TokenType == JsonTokenType.EndObject);
        Debug.Assert(state.Expected.TokenType == JsonTokenType.EndObject);

        return null;
    }

    private static string? CompareProperty(ref ComparatorState state)
    {
        Debug.Assert(state.Expected.TokenType == JsonTokenType.PropertyName);
        Debug.Assert(state.Actual.TokenType == JsonTokenType.PropertyName);
        Debug.Assert(state.Expected.ValueTextEquals(state.Actual.ValueSpan)); // we have already compared the property names

        state.Expected.Read();
        state.Actual.Read();

        var propertyError = CompareToken(ref state);
        if (propertyError is not null) return propertyError;
        return null;
    }

    private static void ReadFullProperty(ref Utf8JsonReader reader)
    {
        var depth = 0;

        do
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                    depth++;
                    break;
                case JsonTokenType.EndObject:
                case JsonTokenType.EndArray:
                    depth--;
                    goto default;
                default:
                    if (depth == 0)
                    {
                        reader.Read();
                        return;
                    }

                    break;
            }
        }
        while (reader.Read());
    }
}