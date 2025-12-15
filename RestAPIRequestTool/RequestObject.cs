using RestSharp;

namespace RestAPIRequestTool;

public class Settings
{
    public string Jwt { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public Method Method { get; set; } = 0;

    public string Resource { get; set; } = string.Empty;

    public List<RequestObject> Requests { get; set; } = new();
}


public class RequestObject
{
    public object? Body { get; set; }

    public List<KVP> QueryParams { get; set; } = new();

    public List<KVP> UrlSegments { get; set; } = new();
}

public class KVP
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}