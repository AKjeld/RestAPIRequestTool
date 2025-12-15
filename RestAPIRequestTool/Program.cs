using RestAPIRequestTool;
using RestSharp;
using System.Text.Json;

var json = await File.ReadAllTextAsync("../../../settings.json");

var logFilePath = "../../../log.txt";

if (File.Exists(logFilePath))
{
    File.Delete(logFilePath);
}

await using StreamWriter writer = new StreamWriter(logFilePath, append: true);

Settings? settingsDeserialized = JsonSerializer.Deserialize<Settings>(json, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

if (settingsDeserialized == null || settingsDeserialized.Requests == null || settingsDeserialized.Requests.Count == 0)
{
    Console.WriteLine("No requests found in the JSON file.");
    return;
}

Settings settings = settingsDeserialized;

#region Setup
var jwt = settings.Jwt;
var uri = new Uri(settings.Url);
Method method = settings.Method;
var resource = settings.Resource;
#endregion

var client = new RestClient(uri);

List<RequestObject> requestObjects = settingsDeserialized.Requests;

foreach (var requestObject in requestObjects)
{
    var request = new RestRequest(resource, method);

    // Add query parameters
    foreach (var kvp in requestObject.QueryParams)
    {
        request.AddQueryParameter(kvp.Key, kvp.Value);
    }

    foreach (var kvp in requestObject.UrlSegments)
    {
        request.AddUrlSegment(kvp.Key, kvp.Value);
    }

    // Add body if present
    if (requestObject.Body != null)
    {
        request.AddStringBody(JsonSerializer.Serialize(requestObject.Body), DataFormat.Json);
    }

    // Add authorization header
    request.AddHeader("Authorization", jwt);

    try
    {
        var response = await client.ExecuteAsync(request);
        Console.WriteLine($"Request to {uri}{resource} completed with status code: {response.StatusCode}");
        await writer.WriteLineAsync($"Request to {uri}{resource} completed with status code: {response.StatusCode}");
        await writer.WriteLineAsync(response.Content ?? string.Empty);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error executing request to {uri} {resource}: {ex.Message}");
        await writer.WriteLineAsync($"Error executing request to {uri} {resource}: {ex.Message}");
    }
}
