using System.Text.Json;
using Microsoft.Extensions.Primitives;

public class SpaceTrimmerMiddleware
{
    private readonly RequestDelegate _next;

    public SpaceTrimmerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
       TrimWhitespace(context.Request);
        
        if (context.Request.ContentLength > 0 && 
            context.Request.ContentType != null && 
            context.Request.ContentType.Contains("application/json"))
        {
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

            // Deserialize the JSON into a JsonDocument
            using var document = JsonDocument.Parse(requestBody);
            var root = document.RootElement;
            var trimmedRoot = TrimSpacesInJsonElement(root);
            
            using var stream = new MemoryStream();
            await using var writer = new Utf8JsonWriter(stream);
            trimmedRoot.WriteTo(writer);
            await writer.FlushAsync();
            var buffer = stream.ToArray();
            
            // Replace the original request body
            context.Request.Body = new MemoryStream(buffer);
        }
    
        await _next(context);
    }

    private void TrimWhitespace(HttpRequest request)
    {
        // Process URL Segments
        var pathSegments = request.Path.Value.Split('/');
        for (var i = 0; i < pathSegments.Length; i++)
        {
            pathSegments[i] = pathSegments[i].Trim();
        }
        request.Path = new PathString(string.Join('/', pathSegments));
        
        var routeValues = request.RouteValues.Select(x => new KeyValuePair<string, object>(x.Key, x.Value.ToString()?.Trim()));
        var routeValuesCollection = new RouteValueDictionary(routeValues.ToDictionary(x => x.Key, x => x.Value));
        request.RouteValues = routeValuesCollection;
        
        var queryParams = request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString().Trim()));
        var queryCollection = new QueryCollection(queryParams.ToDictionary(x => x.Key, x => new StringValues(x.Value)));
        request.Query = queryCollection;
    }
    
    public static JsonElement TrimSpacesInJsonElement(JsonElement element)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        ProcessElement(element, writer);
        writer.Flush();

        stream.Position = 0;
        using var doc = JsonDocument.Parse(stream);
        return doc.RootElement.Clone(); // Clone to return as a JsonElement
    }
    private static void ProcessElement(JsonElement element, Utf8JsonWriter writer)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    ProcessElement(property.Value, writer);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    ProcessElement(item, writer);
                }
                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                writer.WriteStringValue(element.GetString().Trim()); // Trim spaces here
                break;

            default:
                element.WriteTo(writer);
                break;
        }
    }    
}