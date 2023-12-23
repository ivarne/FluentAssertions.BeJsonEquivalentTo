This package provides a new way to compare strings that takes JSON semantics into consideration. It does token comparison using `System.Text.Json.Utf8JsonReader` to compare the actual json tokens. The default behaviour differs from `string.Should().Be()` in the following ways:

* It ignores all whitespace outside of json properties and strings (actual `"{}"` equals expected `"{\n}"`
* It decodes unicode escape sequences (eg. `\u00a3`) before comparing strings.

With options you can loosen the comparison even more

* **IgnorePropertyOrder**: make actual `{"a":1, "b":2}` equal expected `{"b":2, "a":1}`
* **IgnoreExtraProperties**: make actual `{"a":1, "b":2}` equal expected `{"a":1}`
* **IgnoreArrayOrder**: make actual `[1,2,3]` equal expected `[3,2,1]`

You can also pass options to customize the `Utf8JsonReader` settings
* **CommentHandeling**: `Skip`, `Allow`, `Error`.
* **MaxDepth**: Avoid infinate recursion (default 64).
* **AllowTrailingComma**: Allow trailing comma in objects and arrays.

### Usage
A typical usage will be something like (using c# 11/dotnet 7 raw string literals)
```c#
var actual = ...

actual.Should().BeJsonEquivalentTo("""
{
    "a": 123,
    "b": [1,2,3]
}
""")
```

### Justification

Lots of communication and storage in modern apps depends on the json format, thus making it interesting to write assertions that ensures that the json output is the same when the implementation changes. The current alternatives in Fluentassertions is to have exact string comparison with `actual.Should().Be(expected)`, which is problematic because the json format is flexible in a few ways.

* `System.Text.Json` indented json serialization outputs `\r\n` on windows, and `\n` on osx/linux. This means you need to normalize line endings.
* Indented json and non-indented json is usually strictly equivalent, but a string comparison can't see that they are equal. (`"{}"` vs `"{\n}"`)
   * Test file usually use want to use indented json, but the API result is usually unindented.
* Sometimes the order of json properties isn't stable, and test assertions needs to accept that to reduce maintanance and allow cross platform tests.
* Json support unicode escape sequences in strings `"\u00a3"` and for most parsers they are equivalent (unless you paste json in a `<script>` tag in html and get script injection when a json string includes an `</script>` end tag)

The obvious alternative would be to parse the json to C# classes and use `BeEquivalentTo`, but that has its own problems.

* It's dependent on parser settings wich might be different from the actual consumer of the json. (case sensitivity on properties, enums, dates ...)
* Some C# specific behaviour in `BeEquivalentTo` does not make sense for JSON. (Different type system).
* You need to have a C# class that is in sync with the json you test. You can usually reuse the existing model, but that means your tests change unintentionally when you change the model.



### Customization API
TODO: find a good way to provide extra settings.




### Considerations

#### Automatic serialization
It is very possible to make `IsJsonEquivalentTo` work on all objects, not just strings, by just using `JsonSerializer.Serialize()` to convert the arguments to string. Currently the user needs to do that manually

```c#
var actual = ...
var expected = ...

// Current generic code
JsonSerializer.Serialize(actual).Should().BeJsonEquivalentTo(JsonSerializer.Serialize(expected));
// With suggestion, this could be written
actual.Should().BeJsonEquivalentTo(expected)


```




