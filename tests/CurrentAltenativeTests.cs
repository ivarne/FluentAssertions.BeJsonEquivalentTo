
namespace FluentAssertions.JsonEquivalent.Tests;


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