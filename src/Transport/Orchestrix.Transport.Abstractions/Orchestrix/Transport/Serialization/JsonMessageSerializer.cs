namespace Orchestrix.Transport.Serialization;

using System.Text.Json;

/// <summary>
/// JSON-based message serializer using System.Text.Json.
/// </summary>
public class JsonMessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMessageSerializer"/> class.
    /// </summary>
    /// <param name="options">Optional JSON serializer options.</param>
    public JsonMessageSerializer(JsonSerializerOptions? options = null)
    {
        _options = options ?? DefaultOptions;
    }

    /// <inheritdoc />
    public byte[] Serialize<T>(T message)
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, _options);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }

    /// <inheritdoc />
    public object? Deserialize(byte[] data, Type type)
    {
        return JsonSerializer.Deserialize(data, type, _options);
    }
}
