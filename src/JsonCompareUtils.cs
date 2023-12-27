using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FluentAssertions.JsonEquivalent;

public static class JsonCompareUtils
{
    private const int CharCountBeforeError = 14;
    private const int CharCountAfterError = 150;
    public static readonly JsonComparatorOptions DefaultOptions = new();

    public static string? CreateErrorMessage(string message, ref ComparatorState state)
    {
        var actualCharacterPositionError = Encoding.UTF8.GetCharCount(state.ActualBytes.AsSpan(0, state.ActualOffset)); // might be less than pos if there are multi byte characters
        var expectedCharacterPositionError = Encoding.UTF8.GetCharCount(state.ExpectedBytes.AsSpan(0,
            state.ExpectedOffset)); // might be less than pos if there are multi byte characters

        return $"""

                {message}
                {GetRelevantErrorPart(state.ActualOffset, state.ActualBytes)} (diff at index {actualCharacterPositionError})
                {GetRelevantErrorPart(state.ExpectedOffset, state.ExpectedBytes)} (diff at index {expectedCharacterPositionError})
                {"^",CharCountBeforeError + 1}

                """;
    }

    private static readonly Regex RemoveNewlinesAndIndetionRegex = new Regex(@"\r?\n\s*", RegexOptions.Compiled);

    private static string GetRelevantErrorPart(int pos, ReadOnlySpan<byte> bytes)
    {
        // To keep the index of the errors after decoding and removing newlines and indentation
        // we need to take a split the string at the error position and remove newlines and indentation separately
        // before concatenating
        var startBeforePos = Math.Max(0, pos - (CharCountBeforeError * 3)); // take 3 times as many utf8 bytes than characters (might be multi byte characters, and we remove newlines and indentation)
        var lengthAfterPos = Math.Min(CharCountAfterError * 3, bytes.Length - pos); // take 3 times as many utf8 bytes than characters

        var beforeErrorString = Encoding.UTF8.GetString(bytes.Slice(startBeforePos, pos - startBeforePos));
        beforeErrorString = RemoveNewlinesAndIndetionRegex.Replace(beforeErrorString, " ")
            .PadLeft(CharCountBeforeError);
        beforeErrorString = beforeErrorString.Substring(beforeErrorString.Length - CharCountBeforeError);

        var afterErrorString = Encoding.UTF8.GetString(bytes.Slice(pos, lengthAfterPos));
        afterErrorString = RemoveNewlinesAndIndetionRegex.Replace(afterErrorString, " ");
        afterErrorString = afterErrorString.Substring(0, Math.Min(afterErrorString.Length, CharCountAfterError));

        return beforeErrorString + afterErrorString; // concatenate the strings and ensure the error mark is at pos CharCountBeforeError
    }
}