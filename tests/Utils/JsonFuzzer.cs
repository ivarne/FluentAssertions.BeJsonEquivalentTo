

using System.Text;
using System.Text.Encodings.Web;

namespace FluentAssertions.JsonEquivalent.Tests.Utils;

public static class JsonFuzzer
{
    public record JsonFuzzerOptions
    {
        public bool RandomizePropertyOrder { get; set; }
        public bool Escape { get; set; }
    }

    public static string GetFuzzedJson(int seed, JsonFuzzerOptions options)
    {
        // Set the seed for random number generation
        Random random = new Random(seed);

        // Create a MemoryStream to write JSON data to
        using MemoryStream stream = new MemoryStream();
        // Create a Utf8JsonWriter to write JSON data to the MemoryStream
        using Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions()
        {
            Indented = true,
            Encoder = options.Escape ? JavaScriptEncoder.Default : JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        // Call your custom fuzzer method to generate random JSON data
        FuzzJson(writer, random, 5, options.RandomizePropertyOrder);

        // Flush the writer to ensure all data is written to the MemoryStream
        writer.Flush();

        // Convert the MemoryStream to a string for display or further processing
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    static void FuzzJson(Utf8JsonWriter writer, Random random, int depth, bool randomizePropertyOrder)
    {
        if (depth <= 0)
        {
            // If depth is 0, write a simple value (string, number, boolean, etc.)
            switch (random.Next(3))
            {
                case 0:
                    writer.WriteStringValue($"string_{random.Next()}");
                    break;
                case 1:
                    writer.WriteNumberValue(random.NextDouble());
                    break;
                case 2:
                    writer.WriteBooleanValue(random.Next(2) == 0);
                    break;
            }
        }
        else
        {
            // Randomly choose between writing an array or an object
            if (random.Next(2) == 0)
            {
                // Write the start of an array
                writer.WriteStartArray();

                // Determine the number of elements in the array (up to 5 for simplicity)
                int arrayLength = random.Next(0, 6);

                // Write the specified number of elements in the array
                for (int i = 0; i < arrayLength; i++)
                {
                    // Call the fuzzer recursively for each element in the array
                    FuzzJson(writer, random, depth - 1, randomizePropertyOrder);
                }

                // End the array
                writer.WriteEndArray();
            }
            else
            {
                // Write the start of an object
                writer.WriteStartObject();

                // Determine the number of properties in the object (up to 5 for simplicity)
                int propertyCount = random.Next(0, 6);
                var propertyIndexes = Enumerable.Range(0, propertyCount);


                var orderRng = new Random(random.Next());
                if (randomizePropertyOrder)
                {
                    propertyIndexes = propertyIndexes.OrderBy(x => orderRng.Next()).ToArray();
                }

                var newSeed = random.Next(100000);

                // Write the specified number of properties in the object
                foreach (var propertyIndex in propertyIndexes)
                {
                    var propertyRandom = new Random(newSeed + propertyIndex);
                    var propertyName = $"property_{propertyIndex}";
                    // Write the property name
                    writer.WritePropertyName(propertyName);

                    // Call the fuzzer recursively for the property value
                    FuzzJson(writer, propertyRandom, depth - 1, randomizePropertyOrder);
                }

                // End the object
                writer.WriteEndObject();
            }
        }
    }
}
