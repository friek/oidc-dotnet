using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace OpenIDConnectPlayground;

/// <summary>
/// Our own naive implementation of a session ticket store  
/// </summary>
public class TicketStore : ITicketStore
{
    private readonly IDistributedCache _cache;
    private readonly TicketSerializer _ticketSerializer = TicketSerializer.Default;

    public TicketStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var sessionKey = Guid.NewGuid().ToString();
        _cache.SetAsync(sessionKey, _ticketSerializer.Serialize(ticket)).Wait();

        return Task.FromResult(sessionKey);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        return _cache.SetAsync(key, _ticketSerializer.Serialize(ticket));
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var authTicket = _cache.GetAsync(key).Result;
        return authTicket is null
            ? Task.FromResult<AuthenticationTicket?>(null)
            : Task.FromResult(_ticketSerializer.Deserialize(authTicket));
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }
}