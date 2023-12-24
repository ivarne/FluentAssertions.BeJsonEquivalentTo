using System.Diagnostics;
using System.Text.Json;
using FluentAssertions;

namespace FluentAssertions.JsonEquivalent;

public static class JsonCompare
{
    private static readonly JsonComparatorOptions DefaultOptions = new();

    public static JsonCompareError? IsJsonEquivalent(string actual, string expected, JsonComparatorOptions? options = null)
    {
        var actualBytes = System.Text.Encoding.UTF8.GetBytes(actual);
        var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expected);
        return IsJsonEquivalent(actualBytes, expectedBytes, options);
    }

    public static JsonCompareError? IsJsonEquivalent(byte[] actualBytes, byte[] expectedBytes, JsonComparatorOptions? options = null)
    {
        options ??= DefaultOptions;
        var actual = new Utf8JsonReader(actualBytes, options.Value);
        var expected = new Utf8JsonReader(expectedBytes, options.Value);
        var comparisonResult = CompareReaders(ref actual, actualBytes, ref expected, expectedBytes, options.Value);
        if (comparisonResult is not null)
        {
            return comparisonResult;
        }

        if (actual.Read() == false && expected.Read() == false)
        {
            return null;
        }

        return CreateErrorMessage("end of stream mismatch", ref actual, actualBytes, ref expected, expectedBytes);
    }

    private static JsonCompareError? CompareReaders(ref Utf8JsonReader actual, byte[] actualBytes,
        ref Utf8JsonReader expected, byte[] expectedBytes, JsonComparatorOptions options)
    {
        int depth = 0;
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
                    depth++;
                    if (options.LooseObjectOrderComparison)
                    {
                        var objectCompareResult = CompareObjectIgnoreOrder(ref actual, actualBytes, ref expected, expectedBytes, options);
                        if (objectCompareResult is not null)
                        {
                            return objectCompareResult;
                        }
                    }

                    break;
                case JsonTokenType.EndObject:
                    if (depth-- == 0)
                    {
                        return null;
                    }

                    break;
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

    private static JsonCompareError? CompareObjectIgnoreOrder(ref Utf8JsonReader actual, byte[] actualBytes,  ref Utf8JsonReader expected, byte[] expectedBytes, JsonComparatorOptions options)
    {
        var cacheActual = new Dictionary<string, ReadOnlyMemory<byte>>();
        actual.Read();
        expected.Read();
        while (true)
        {
            switch ((actual.TokenType, expected.TokenType))
            {
                case (JsonTokenType.EndObject, JsonTokenType.EndObject):
                    return cacheActual.Count == 0 ? null : CreateErrorMessage("Different counts", ref actual, actualBytes, ref expected, expectedBytes);
                case (JsonTokenType.EndObject, JsonTokenType.PropertyName):
                case (JsonTokenType.PropertyName, JsonTokenType.EndObject):
                    return CreateErrorMessage("Different property counts", ref actual, actualBytes, ref expected, expectedBytes);
                case (JsonTokenType.PropertyName, JsonTokenType.PropertyName):
                    break; // Everything else returns.
                default:
                    return CreateErrorMessage("Should not happen!", ref actual, actualBytes, ref expected, expectedBytes);
            }
            if (actual.ValueTextEquals(expected.ValueSpan))
            {
                // Properties actually match. Do the easy path.
                actual.Read();
                expected.Read();
                var recursiveCompareResult =
                    CompareReaders(ref actual, actualBytes, ref expected, expectedBytes, options);
                if (recursiveCompareResult is not null)
                {
                    return recursiveCompareResult;
                }

                actual.Read();
                expected.Read();
            }
            else
            {
                // read one property in actual into cache
                var actualPropertyName = actual.GetString()!;
                var propertyContent = GetPropertyContent(ref actual, actualBytes);
                cacheActual.Add(actualPropertyName, propertyContent);
            }
        }
    }

    private static ReadOnlyMemory<byte> GetPropertyContent(ref Utf8JsonReader actual, byte[] actualBytes)
    {
        var start = (int)actual.TokenStartIndex;
        ReadUntillObjectEnd(ref actual);
        var end = (int)actual.TokenStartIndex;

        return actualBytes.AsMemory(start, end - start);
    }

    private static void ReadUntillObjectEnd(ref Utf8JsonReader reader)
    {
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    ReadUntillObjectEnd(ref reader);
                    break;
                case JsonTokenType.EndObject:
                    return;
            }
        }

        throw new Exception("Something failed");
    }
}