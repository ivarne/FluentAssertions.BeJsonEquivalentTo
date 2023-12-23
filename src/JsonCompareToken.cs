using System.Diagnostics;
using System.Text.Json;
using FluentAssertions;

namespace JsonCompare;

public static class JsonCompareToken
{
    private static readonly JsonComparatorOptions DefaultOptions = new();

    public static JsonCompareError? IsJsonTokenEquivalent(string actual, string expected, JsonComparatorOptions? options = null)
    {
        var actualBytes = System.Text.Encoding.UTF8.GetBytes(actual);
        var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
        return IsJsonTokenEquivalent(actualBytes, expectedBytes, options);
    }

    public static JsonCompareError? IsJsonTokenEquivalent(byte[] actualBytes, byte[] expectedBytes, JsonComparatorOptions? options = null)
    {
        options ??= DefaultOptions;
        var actual = new Utf8JsonReader(actualBytes, options.Value);
        var expected = new Utf8JsonReader(expectedBytes, options.Value);
        var comparisonResult = CompareReaders(ref actual, actualBytes, ref expected, expectedBytes, options.Value);
        return comparisonResult;
    }

    private static JsonCompareError? CompareReaders(ref Utf8JsonReader actual, byte[] actualBytes,
        ref Utf8JsonReader expected, byte[] expectedBytes, JsonComparatorOptions options)
    {
        while (actual.Read() && expected.Read())
        {
            if (actual.TokenType != expected.TokenType)
            {
                return CreateErrorMessage("JsonTokenType mismatch", ref actual, actualBytes, ref expected, expectedBytes);
            }

            switch (actual.TokenType)
            {
                // Ignore tokens without value
                case JsonTokenType.StartObject:
                case JsonTokenType.EndObject:
                case JsonTokenType.None:
                case JsonTokenType.StartArray:
                case JsonTokenType.EndArray:
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Null:
                    break;
                // compare the value of tokens with value
                case JsonTokenType.PropertyName:
                    if (!actual.ValueTextEquals(expected.ValueSpan))
                    {
                        return CreateErrorMessage("PropertyName mismatch", ref actual, actualBytes, ref expected,
                            expectedBytes);
                    }

                    break;
                case JsonTokenType.String:
                    if (!actual.ValueTextEquals(expected.ValueSpan))
                    {
                        return CreateErrorMessage("string mismatch", ref actual, actualBytes, ref expected, expectedBytes);
                    }

                    break;
                case JsonTokenType.Comment: // compare comments if they are not ignored
                    if (actual.GetComment() != expected.GetComment())
                    {
                        return CreateErrorMessage("comment mismatch", ref actual, actualBytes, ref expected, expectedBytes);
                    }

                    break;
                case JsonTokenType.Number:
                    if (actual.GetDecimal() != expected.GetDecimal())
                    {
                        return CreateErrorMessage("number mismatch", ref actual, actualBytes, ref expected, expectedBytes);
                    }

                    break;
            }
        }

        return null;
    }

    private static JsonCompareError CreateErrorMessage(string message, ref Utf8JsonReader actual, ReadOnlySpan<byte> actualBytes,
        ref Utf8JsonReader expected, ReadOnlySpan<byte> expectedBytes)
    {
        return new JsonCompareError(message, GetRelevantErrorPart(ref actual, actualBytes),
            GetRelevantErrorPart(ref expected, expectedBytes), "               ^");
    }

    private static string GetRelevantErrorPart(ref Utf8JsonReader reader, ReadOnlySpan<byte> bytes)
    {                      
        var pos = reader.TokenStartIndex;
        // utf-8 is a variable length encoding, so we need to find the start of a char for a split
        var startPos = (int)Math.Max(0, pos - 15);
        while((bytes[startPos] & 0xC0) == 0x80)
        {
            startPos--;
        }

        // Take about 75 characters (less if there are multibyte characters)
        var length = Math.Min(75, bytes.Length - startPos - 1);
        while ((bytes[startPos + length] & 0xC0) == 0x80)
        {
            length--;
        }
        
        return $"{System.Text.Encoding.UTF8.GetCharCount(bytes.Slice(0,(int)reader.TokenStartIndex))} {System.Text.Encoding.UTF8.GetString(bytes.Slice(startPos, length))}";
    }
}