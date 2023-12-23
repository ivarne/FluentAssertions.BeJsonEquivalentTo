namespace JsonCompare;

using System.Text.Json;

public class JsonCompareAnyOrder
{
    private readonly byte[] _actual;
    private readonly byte[] _expected;
    private readonly JsonReaderOptions _options;

    private static readonly JsonReaderOptions DefaultOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public JsonCompareAnyOrder(string actual, string expected) : this(actual, expected, DefaultOptions) { }

    public JsonCompareAnyOrder(string actual, string expected, JsonReaderOptions options)
    {
        _actual = System.Text.Encoding.UTF8.GetBytes(actual);
        _expected = System.Text.Encoding.UTF8.GetBytes(expected);
        _options = options;
    }

    public bool Compare()
    {
        var actual = new Utf8JsonReader(_actual, _options);
        var expected = new Utf8JsonReader(_expected, _options);
        return Compare(ref actual, ref expected);
    }

    private bool Compare(ref Utf8JsonReader actual, ref Utf8JsonReader expected)
    {
        while (actual.Read() && expected.Read())
        {
            if (actual.TokenType != expected.TokenType)
            {
                return false;
            }

            switch (actual.TokenType)
            {
                case JsonTokenType.StartObject:
                    var obj = CompareObject(ref actual, ref expected);
                    if (!obj) return false;
                    break;
                case JsonTokenType.StartArray:
                    obj = CompareArray(ref actual, ref expected);
                    if (!obj) return false;
                    break;
                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                    return true;
                case JsonTokenType.PropertyName:
                    throw new Exception("should not happen");
                default:
                    if (!actual.ValueTextEquals(expected.ValueSpan))
                    {
                        return false;
                    }

                    break;
            }
        }

        return true;
    }

    private bool CompareArray(ref Utf8JsonReader actual, ref Utf8JsonReader expected)
    {
        while (actual.Read() && expected.Read())
        {
            if (actual.TokenType != expected.TokenType)
            {
                return false;
            }

            switch (actual.TokenType)
            {
                case JsonTokenType.StartObject:
                    var obj = CompareObject(ref actual, ref expected);
                    if (!obj) return false;
                    break;
                case JsonTokenType.StartArray:
                    obj = CompareArray(ref actual, ref expected);
                    if (!obj) return false;
                    break;
                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                    return true;
                case JsonTokenType.PropertyName:
                    throw new Exception("should not happen");
                default:
                    if (!actual.ValueTextEquals(expected.ValueSpan))
                    {
                        return false;
                    }

                    break;
            }
        }

        throw new Exception("NotPossible");
    }

    private bool CompareObject(ref Utf8JsonReader actual, ref Utf8JsonReader expected)
    {
        var lookup = new Dictionary<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>();
        while (actual.Read() && expected.Read())
        {
            switch (actual.TokenType, expected.TokenType)
            {
                case (JsonTokenType.PropertyName, JsonTokenType.PropertyName):
                    break;
                case (JsonTokenType.EndObject, JsonTokenType.EndObject):
                    return true;
                case (JsonTokenType.PropertyName, JsonTokenType.EndObject):
                case (JsonTokenType.EndObject, JsonTokenType.PropertyName):
                    return false; // One object ends before the other is fully read
                default:
                    throw new Exception("This shouldn't happen");
            }

            if (actual.ValueTextEquals(expected.ValueSpan))
            {
                var recursiveResult = Compare(ref actual, ref expected);
                if (!recursiveResult)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        throw new Exception("Tried to read past the end of the reader inside an object");
    }
}