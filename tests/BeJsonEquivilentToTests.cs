namespace JsonCompare.Tests;


public class TestJsonCompare
{
    [Fact]
    public void TestEqual()
    {
        var actual = """{"name":"John", "number": 3.141592, "emptyArray":[], "array":[null, 1, 3.141592, "", "string", {}, {"t":null}]}""";

        actual.Should().BeJsonEquivalentTo(actual);
    }

    [Fact]
    public void TestEncodingIndiferenceInValue()
    {
        var safe = """{"name":"O\u0027Reilly\uD83D\uDCDA"}""";
        var nonSafe = """{"name":"O'Reilly📚"}""";
        safe.Should().BeJsonEquivalentTo(nonSafe);
    }

    [Fact]
    public void TestEncodingIndiferenceInKey()
    {
        var safe = """{"name of \uD83D\uDCDA":"Book name"}""";
        var nonSafe = """{"name of 📚":"Book name"}""";
        safe.Should().BeJsonEquivalentTo(nonSafe);
    }

    [Fact]
    public void TestTrailingComma()
    {
        var withComma = """{"a":[1,],}""";
        var noComma = """{"a":[1]}""";
        withComma.Should().BeJsonEquivalentTo(noComma);
    }

    [Fact]
    public void TestTrailingCommaConfigurable()
    {
        var withComma = """{"a":[1,],}""";
        var noComma = """{"a":[1]}""";
        var action = () =>
        withComma.Should().BeJsonEquivalentTo(noComma, new JsonComparatorOptions()
        {
            AllowTrailingCommas = false,
        });
        action.Should().Throw<JsonException>().WithMessage("The JSON array contains a trailing comma at the end which is not supported in this mode. Change the reader options.*");
    }

    [Fact]
    public void Test2FormattedWithComment()
    {
        var actual = """{"name":"John"}""";
        var expected = """
                       {
                          "name":"John"
                          // This is a comment that isn't compared
                       }
                       """;
        actual.Should().BeJsonEquivalentTo(expected);
    }

    [Fact]
    public void TestCommentCommentComparison()
    {
        var actual = """
                     {
                     "name":"John"// This is a comment that isn't compared
                     }
                     """;
        var expected = """
                       {
                          "name":"John"
                          // This is a comment that isn't compared
                       }
                       """;
        actual.Should().BeJsonEquivalentTo(expected, new JsonComparatorOptions()
        {
            CommentHandling = JsonCommentHandling.Allow
        });
    }

    [Fact]
    public void Test()
    {
        var t1 = """{"name": "Joe Doe", "age": 15488}""";
        var t2 = """{"name": "Joe Doe", "age": 15488}// a comment""";
        t1.Should().BeJsonEquivalentTo(t2, new JsonComparatorOptions() { CommentHandling = JsonCommentHandling.Skip });
    }
}