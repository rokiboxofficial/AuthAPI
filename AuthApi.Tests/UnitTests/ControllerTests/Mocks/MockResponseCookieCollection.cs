using Microsoft.AspNetCore.Http;

namespace AuthApi.Tests.UnitTests.ControllerTests.Mocks;

internal class MockResponseCookieCollection : IResponseCookies
{
    public string? Key { get; set; }
    public string? Value { get; set; }
    public CookieOptions? Options { get; set; }
    public int Count { get; set; }

    public void Append(string key, string value, CookieOptions options)
    {
        Key = key;
        Value = value;
        Options = options;
        Count++;
    }

    public void Append(string key, string value)
    {
        throw new NotImplementedException();
    }

    public void Delete(string key, CookieOptions options)
    {
        throw new NotImplementedException();
    }

    public void Delete(string key)
    {
        throw new NotImplementedException();
    }
}