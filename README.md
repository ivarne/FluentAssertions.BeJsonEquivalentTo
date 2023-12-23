Lots of communication and storage in modern apps depends on the json format, thus making it interesting to write assertions that ensures that the json output is the same when the implementation changes. The current alternatives in Fluentassertions is to have exact string comparison with `actual.Should().Be(expected)`, which is problematic because the json format is flexible in a few ways.

* `System.Text.Json` indented json serialization outputs `\r\n` on windows, and `\n` on osx/linux. This makes it hard to have cross platform tests
* Indented json and non-indented json is usually strictly equivalent, but a string comparison can't see that they are equal. (`"{}"` vs `"{\n}"`)
* Sometimes the order of json properties isn't stable, and test assertions needs to accept that.
* Json support unicode escape sequences in strings `"\u00a3"` and for most parsers they are equivalent (unless you paste json in a `<script>` tag in html and get script injection when a json string includes an `</script>` end tag)

* 




```

```
