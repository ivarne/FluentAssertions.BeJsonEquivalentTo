using System.Text.Json;

namespace FluentAssertions.JsonEquivalent;

public readonly struct JsonComparatorOptions
{
    public JsonComparatorOptions()
    {
    }

    public readonly JsonCommentHandling CommentHandling { get; init; } = JsonCommentHandling.Skip;
    public bool AllowTrailingCommas { get; init; } = true;
    public int MaxDepth { get; init; } = 64;

    public bool LooseObjectOrderComparison { get; init; } = false;

    public static implicit operator JsonReaderOptions(JsonComparatorOptions options) => new JsonReaderOptions()
    {
        CommentHandling = options.CommentHandling,
        AllowTrailingCommas = options.AllowTrailingCommas,
        MaxDepth = options.MaxDepth,
    };

}