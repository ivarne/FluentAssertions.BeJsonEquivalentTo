namespace FluentAssertions.JsonEquivalent.Tests;

public class TheoryTests
{
    public class TestCase
    {
        public required string Name { get; set; }
        public required string Actual { get; set; }
        public required string Expected { get; set; }
        public required bool Throws_Be { get; set; }
        public required bool Throws_BeEquivalentTo { get; set; }
        public required bool Throws_BeJsonEquivalentTo { get; set; }

    }

    public static IEnumerable<object[]> GetTestCases => new[]
    {
        new[]
        {
            new TestCase
            {
                Name = "TestBinaryEqual",
                Actual =
                    """{"name":"John", "number": 3.141592, "emptyArray":[], "array":[null, 1, 3.141592, "", "string", {}, {"t":null}]}""",
                Expected =
                    """{"name":"John", "number": 3.141592, "emptyArray":[], "array":[null, 1, 3.141592, "", "string", {}, {"t":null}]}""",
                Throws_Be = false,
                Throws_BeEquivalentTo = false,
                Throws_BeJsonEquivalentTo = false
            },
        },
        new[]
        {
            new TestCase
            {
                Name = "TestEncodingIndiferenceInValue",
                Actual = """{"name":"O\u0027Reilly\uD83D\uDCDA"}""",
                Expected = """{"name":"O'Reilly📚"}""",
                Throws_Be = true,
                Throws_BeEquivalentTo = true,
                Throws_BeJsonEquivalentTo = false
            },
        },
    };

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void Test_BeEquivalentTo(TestCase testCase)
    {
        var action = () => testCase.Actual.Should().BeEquivalentTo(testCase.Expected);
        if (testCase.Throws_BeEquivalentTo)
        {
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }
        else
        {
            action();
        }
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void Test_Be(TestCase testCase)
    {
        var action = () => testCase.Actual.Should().BeEquivalentTo(testCase.Expected);
        if (testCase.Throws_Be)
        {
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }
        else
        {
            action();
        }
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void TestJsonEquivalent(TestCase testCase)
    {
        var action = () => testCase.Actual.Should().BeJsonEquivalentTo(testCase.Expected);
        if (testCase.Throws_BeJsonEquivalentTo)
        {
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }
        else
        {
            action();
        }
    }
}


public class CurrentAlternativeTests
{
    public class LineEndings
    {
        public string actual = "{\n    \"A\":1,\n    \"B\":2\n}";
        public string expected = "{\r\n    \"A\":1,\r\n    \"B\":2\r\n}";

        public class Test
        {
            public int A { get; set; }
            public int B { get; set; }
        }

        [Fact]
        public void TestBe_ErrorsForDifferentLineEndings()
        {

            var action = () => actual.Should().Be(expected);
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }

        [Fact]
        public void TestBeEquivalentTo_ConsideresLineEndingsAsEqual()
        {
            var action = () => actual.Should().BeEquivalentTo(expected);
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }

        [Fact]
        public void TestBeJsonEquivalent_ConsideresLineEndingsAsEqual()
        {
            actual.Should().BeJsonEquivalentTo(expected);
        }

        [Fact]
        public void TestDeserialized_ConsideresLineEndingsAsEqual()
        {
            var actualObject = JsonSerializer.Deserialize<Test>(actual);
            var expectedObject = JsonSerializer.Deserialize<Test>(expected);
            actualObject.Should().BeEquivalentTo(expectedObject, options => options);
        }
    }

    public class DoubleSpace
    {
        public string actual = "{  }";
        public string expected = "{ }";

        [Fact]
        public void TestBe_ErrorsForDifferentDoubleSpace()
        {
            var action = () => actual.Should().Be(expected);
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }

        [Fact]
        public void TestBeEquivalentTo_ConsideresDoubleSpaceAsEqual()
        {
            var action = () => actual.Should().BeEquivalentTo(expected);
            action
                .Should()
                .Throw<Xunit.Sdk.XunitException>();
        }

        [Fact]
        public void TestBeJsonEquivalent_ConsideresDoubleSpaceAsEqual()
        {
            actual.Should().BeJsonEquivalentTo(expected);
        }
    }
}