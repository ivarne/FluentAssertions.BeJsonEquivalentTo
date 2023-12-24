using System.Text;

namespace FluentAssertions.JsonEquivalent.Tests;

/// <summary>
/// Simple tests to check how sliced multibyte UTF8 bytes are interpreted as strings.
/// </summary>
public class Utf8ToStringTests
{
    [Fact]
    public void Utf8ToString()
    {
        var utf8Bytes = Encoding.UTF8.GetBytes("$\u00a3ह\ud800\udf48").AsSpan();
        utf8Bytes.Length.Should().Be(10);
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 0)).Should().Be("");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 1)).Should().Be("$");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 2)).Should().Be("$\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 3)).Should().Be("$\u00a3");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 4)).Should().Be("$\u00a3\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 5)).Should().Be("$\u00a3\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 6)).Should().Be("$\u00a3ह");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 7)).Should().Be("$\u00a3ह\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 8)).Should().Be("$\u00a3ह\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 9)).Should().Be("$\u00a3ह\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(0, 10)).Should().Be("$\u00a3ह\ud800\udf48");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 0)).Should().Be("");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 1)).Should().Be("\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 2)).Should().Be("\u00a3");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 3)).Should().Be("\u00a3\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 4)).Should().Be("\u00a3\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 5)).Should().Be("\u00a3ह");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 6)).Should().Be("\u00a3ह\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 7)).Should().Be("\u00a3ह\ufffd");
        Encoding.UTF8.GetString(utf8Bytes.Slice(1, 8)).Should().Be("\u00a3ह\ufffd");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(1, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(2, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(3, 10)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(4, 10)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(5, 10)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(6, 10)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(7, 10)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(8, 10)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 0)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 1)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 2)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 3)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 4)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 5)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 6)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 7)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 8)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 9)).Should().Be("\u00a3ह\ud800\udf48");
        // Encoding.UTF8.GetString(utf8Bytes.Slice(9, 10)).Should().Be("\u00a3ह\ud800\udf48");

    }
}