using System.Text.Json;

namespace FluentAssertions.JsonEquivalent;

public ref struct ComparatorState
{
    public Utf8JsonReader Actual;
    public Utf8JsonReader Expected;
    public readonly JsonComparatorOptions Options;
    public readonly byte[] ActualBytes;
    public readonly byte[] ExpectedBytes;
    private readonly int _actualOffset;
    private readonly int _expectedOffset;
    public int ActualOffset => (int)Actual.TokenStartIndex + _actualOffset;
    public int ExpectedOffset => (int)Expected.TokenStartIndex + _expectedOffset;

    public ComparatorState(byte[] actualBytes, byte[] expectedBytes, JsonComparatorOptions options)
    {
        ActualBytes = actualBytes;
        ExpectedBytes = expectedBytes;
        Options = options;
        _actualOffset = 0;
        _expectedOffset = 0;
        Actual = new Utf8JsonReader(ActualBytes, options);
        Expected = new Utf8JsonReader(ExpectedBytes, options);
    }

    public ComparatorState(ref ComparatorState state, int expectedOffset)
    {
        ActualBytes = state.ActualBytes;
        ExpectedBytes = state.ExpectedBytes;
        Options = state.Options;
        _actualOffset = state._actualOffset;
        _expectedOffset = expectedOffset;
        Expected = new Utf8JsonReader(ExpectedBytes.AsSpan(expectedOffset), Options);
        Expected.Read(); // don't start at the None token
        Actual = state.Actual;
    }
}